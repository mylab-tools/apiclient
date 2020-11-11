using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
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
        HttpContent Create(object source);
    }

    class StringHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source)
        {
            return new StringContent(source?.ToString() ?? string.Empty);
        }
    }

    class JsonHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source)
        {
            var content = new StringContent(JsonConvert.SerializeObject(source))
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

            var len = content.Headers.ContentLength.GetValueOrDefault();

            return content;
        }
    }

    class XmlHttpContentFactory : IHttpContentFactory
    {
        public HttpContent Create(object source)
        {
            string strContent = null;

            if (source != null)
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                var ser = new XmlSerializer(source.GetType());
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Encoding = new UTF8Encoding(false)
                };
                using (var mem = new MemoryStream())
                using (var wrtr = XmlWriter.Create(mem, settings))
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
        public HttpContent Create(object source)
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
        public HttpContent Create(object source)
        {
            string content = string.Empty;

            if (source != null)
            {
                var queryBuilder = new StringBuilder();
                var props = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var prop in props)
                {
                    var val = prop.GetValue(source);
                    queryBuilder.Append($"&{prop.Name}={Uri.EscapeDataString(val.ToString())}");
                }

                content = queryBuilder.ToString().TrimStart('&');
            }

            var httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return httpContent;
        }
    }
}