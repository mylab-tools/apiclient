using System.Net.Http;
using System.Net.Http.Formatting;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Creates <see cref="HttpContent"/>
    /// </summary>
    public interface IHttpContentFactory
    {
        HttpContent Create(object source);
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
        public XmlHttpContentFactory() : base(new XmlMediaTypeFormatter())
        {
        }
    }

    class UrlFormHttpContentFactory : ObjectBasedHttpContentFactory
    {
        public UrlFormHttpContentFactory() : base(new FormUrlEncodedMediaTypeFormatter())
        {
        }
    }
}