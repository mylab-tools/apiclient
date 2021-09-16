using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MyLab.ApiClient
{
    interface IParameterApplier
    {
        void Apply(HttpRequestMessage request);
    }

    class UrlParameterApplier : IParameterApplier
    {
        private readonly UrlRequestParameterDescription _description;
        private readonly IApiRequestParameterValueProvider _valueProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="UrlParameterApplier"/>
        /// </summary>
        public UrlParameterApplier(UrlRequestParameterDescription description, IApiRequestParameterValueProvider valueProvider)
        {
            _description = description;
            _valueProvider = valueProvider;
        }

        public void Apply(HttpRequestMessage request)
        {
            request.RequestUri = _description.Modifier.Modify(
                request.RequestUri,
                _description.Name,
                _valueProvider.GetValue());
        }
    }

    class HeaderParameterApplier : IParameterApplier
    {
        private readonly HeaderRequestParameterDescription _description;
        private readonly IApiRequestParameterValueProvider _valueProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderParameterApplier"/>
        /// </summary>
        public HeaderParameterApplier(HeaderRequestParameterDescription description, IApiRequestParameterValueProvider valueProvider)
        {
            _description = description;
            _valueProvider = valueProvider;
        }

        public void Apply(HttpRequestMessage request)
        {
            var val = ObjectToStringConverter.ToString(_valueProvider.GetValue());
            request.Headers.Add(_description.Name, val);
        }
    }

    class ContentParameterApplier : IParameterApplier
    {
        readonly ContentRequestParameterDescription _description;
        readonly IApiRequestParameterValueProvider _valueProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="ContentParameterApplier"/>
        /// </summary>
        public ContentParameterApplier(ContentRequestParameterDescription description, IApiRequestParameterValueProvider valueProvider)
        {
            _description = description;
            _valueProvider = valueProvider;
        }

        public void Apply(HttpRequestMessage request)
        {
            request.Content = _description.ContentFactory.Create(_valueProvider.GetValue());
        }
    }

    class HeaderCollectionParameterApplier : IParameterApplier
    {
        private readonly IApiRequestParameterValueProvider _valueProvider;

        public HeaderCollectionParameterApplier(IApiRequestParameterValueProvider valueProvider)
        {
            _valueProvider = valueProvider;
        }

        public void Apply(HttpRequestMessage request)
        {
            var headers = (IDictionary<string, object>) _valueProvider.GetValue();

            if(headers == null) return;

            foreach (var header in headers)
            {
                var val = ObjectToStringConverter.ToString(header.Value);
                request.Headers.Add(header.Key, val);
            }

        }
    }
}
