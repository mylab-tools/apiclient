using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;

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

    class ObjectBasedHttpContentFactory : IHttpContentFactory
    {
        private readonly MediaTypeFormatter _mediaTypeFormatter;

        protected ObjectBasedHttpContentFactory(MediaTypeFormatter mediaTypeFormatter)
        {
            _mediaTypeFormatter = mediaTypeFormatter;
        }

        public HttpContent Create(object source)
        {
            return new ObjectContent(source.GetType(), source, _mediaTypeFormatter);
        }
    }

    class JsonHttpContentFactory : ObjectBasedHttpContentFactory
    {
        public JsonHttpContentFactory() : base(new JsonMediaTypeFormatter())
        {   
        }
    }

    class XmlHttpContentFactory : ObjectBasedHttpContentFactory
    {
        public XmlHttpContentFactory() : base(new XmlMediaTypeFormatter{UseXmlSerializer = true})
        {
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