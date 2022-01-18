using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Creates <see cref="HttpContent"/>
    /// </summary>
    public interface IHttpContentFactory
    {
        /// <summary>
        /// Creates content object
        /// </summary>
        HttpContent Create(object source, RequestFactoringSettings? settings);
    }

    class StringHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source, RequestFactoringSettings? settings)
        {
            return new StringContent(ObjectToStringConverter.ToString(source));
        }
    }

    class JsonHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source, RequestFactoringSettings? settings)
        {
            var serialized = settings?.JsonSettings != null 
                ? JsonConvert.SerializeObject(source, settings.JsonSettings)
                : JsonConvert.SerializeObject(source);

            var content = new StringContent(serialized)
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

            return content;
        }
    }

    class XmlHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source, RequestFactoringSettings? settings)
        {
            string strContent = null;

            if (source != null)
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                var ser = new XmlSerializer(source.GetType());
                var xmlSettings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Encoding = new UTF8Encoding(false)
                };
                using (var mem = new MemoryStream())
                using (var wrtr = XmlWriter.Create(mem, xmlSettings))
                {

                    ser.Serialize(wrtr, source, ns);
                    wrtr.Flush();
                    
                    strContent = Encoding.UTF8.GetString(mem.ToArray());
                }
            }
            else
            {
                strContent = string.Empty;
            }

            return new StringContent(strContent)
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/xml") }
            };
        }
    }

    class BinaryHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source, RequestFactoringSettings? settings)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if(source is byte[] binSource)
            {
                var resContent = new ByteArrayContent(binSource);
                resContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return resContent;
            }
            else
            {
                throw new NotSupportedException($"Only '{typeof(byte[]).FullName}' supported as binary argument");
            }
        }
    }

    class UrlFormHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source, RequestFactoringSettings? settings)
        {
            string content = string.Empty;

            if (source != null)
            {
                var queryBuilder = new StringBuilder();
                var props = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var prop in props)
                {
                    var formItemAttr = prop.GetCustomAttribute<UrlFormItemAttribute>();

                    string name = formItemAttr?.Name ?? prop.Name;

                    var val = prop.GetValue(source);
                    queryBuilder.Append($"&{name}={Uri.EscapeDataString(val?.ToString() ?? string.Empty)}");
                }

                content = queryBuilder.ToString().TrimStart('&');
            }

            var httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return httpContent;
        }
    }

    class MultipartFormHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source, RequestFactoringSettings? settings)
        {
            var parameter = (IMultipartContentParameter) source;

            var httpContent = new MultipartFormDataContent();

            parameter.AddParts(httpContent);

            return httpContent;
        }
    }
}