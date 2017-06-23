using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RedCucumber.Wac
{
    class WebApiRequestFactory
    {
        private readonly WebApiMethodDescription _methodDescription;
        private readonly string _baseUrl;

        public WebApiRequestFactory(
            WebApiMethodDescription methodDescription,
            string baseUrl)
        {
            _methodDescription = methodDescription;
            _baseUrl = baseUrl;
        }

        public HttpRequestMessage CreateMessage(InvokeParameters invokeParameters)
        {
            var uri = CreateUri(invokeParameters);
            var req = new HttpRequestMessage(
                ConvertHttpMethod(_methodDescription.HttpMethod),
                new Uri(new Uri(_baseUrl), _methodDescription.RelPath))
            {
                RequestUri = uri
            };

            if (_methodDescription.HttpMethod != HttpMethod.Get &&
                _methodDescription.Parameters != null)
            {
                var payloadParam = _methodDescription.Parameters.Values
                    .FirstOrDefault(p => p.Type == WebApiParameterType.Payload);
                if (payloadParam != null)
                {
                    var payload = invokeParameters[payloadParam.MethodParameterName];

                    if(payload != null)
                        InitPayloadContent(req, payload);
                }
                else
                {
                    var formParameters = _methodDescription.Parameters.Values
                        .Where(p => p.Type == WebApiParameterType.FormItem)
                        .ToArray();
                    if (formParameters.Length != 0)
                        InitFormContent(req, formParameters,
                            invokeParameters);
                }
            }

            return req;
        }

        private void InitFormContent(
            HttpRequestMessage req, 
            WebApiParameterDescription[] formParameters,
            InvokeParameters invokeParameter)
        {
            switch (_methodDescription.ContentType)
            {
                case ContentType.FromData:
                    throw new PrepareRequestException("'Form' data not supported yet. Please, use 'UrlEncodedForm' instead.");
                case ContentType.UrlEncodedForm:
                    req.Content = new FormUrlEncodedContent(CreateForm(formParameters, invokeParameter));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_methodDescription.ContentType), _methodDescription.ContentType, null);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> CreateForm(
            WebApiParameterDescription[] formParameters, 
            InvokeParameters invokeParameters)
        {
            var dict = new Dictionary<string, string>();
            foreach (var fp in formParameters)
            {
                dict.Add(fp.Name,
                    invokeParameters[fp.MethodParameterName].ToString());
            }

            return dict;
        }

        private void InitPayloadContent(
            HttpRequestMessage req, 
            object payload)
        {
            switch (_methodDescription.ContentType)
            {
                case ContentType.FromData:
                    throw new PrepareRequestException("'Form' data not supported yet. Please, use 'UrlEncodedForm' instead.");
                case ContentType.UrlEncodedForm:
                    req.Content = new FormUrlEncodedContent(PayloadToForm(payload));
                    break;
                case ContentType.Text:
                case ContentType.Html:
                case ContentType.Javascript:
                    req.Content = new StringContent(payload.ToString());
                    break;
                case ContentType.Xml:
                    req.Content = new StringContent(PayloadToXml(payload));
                    break;
                case ContentType.Json:
                    req.Content = new StringContent(PayloadToJson(payload));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_methodDescription.ContentType), _methodDescription.ContentType, null);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> PayloadToForm(object payload)
        {
            var props = payload.GetType().GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return props.ToDictionary(p => p.Name, p => p.GetValue(payload)?.ToString());
        }

        private string PayloadToJson(object payload)
        {
            var jsonS = new JsonSerializer();

            using (StringWriter textWriter = new StringWriter())
            {
                jsonS.Serialize(textWriter, payload);
                return textWriter.ToString();
            }
        }

        private string PayloadToXml(object payload)
        {
            var ser = new XmlSerializer(payload.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                ser.Serialize(textWriter, payload);
                return textWriter.ToString();
            }
        }

        private Uri CreateUri(InvokeParameters invokeParameters)
        {
            var b = new UriBuilder(new Uri(new Uri(_baseUrl), _methodDescription.RelPath));
            var query = new StringBuilder();

            if (_methodDescription.Parameters != null)
            {
                foreach (var p in _methodDescription.Parameters.Values)
                {
                    if (p.Type == WebApiParameterType.Get)
                    {
                        if (query.Length != 0)
                            query.Append("&");
                        query.AppendFormat("{0}={1}", 
                            Uri.UnescapeDataString(p.Name),
                            Uri.UnescapeDataString(invokeParameters[p.MethodParameterName].ToString()));
                    }
                }
            }

            
            b.Query = query.ToString();

            return b.Uri;
        }

        System.Net.Http.HttpMethod ConvertHttpMethod(HttpMethod method)
        {
            return new System.Net.Http.HttpMethod(method.ToString().ToUpper());
        }
    }
}