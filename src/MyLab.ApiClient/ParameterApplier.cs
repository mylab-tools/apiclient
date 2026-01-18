using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            var valObj = _valueProvider.GetValue();

            if (_description.Name == "If-Modified-Since")
            {
                Debug.WriteLine("!!!! " + valObj.GetType().FullName);

                if (valObj is DateTime dt)
                {
                    request.Headers.IfModifiedSince = new DateTimeOffset(dt);
                }
                else if (valObj is DateTimeOffset dtOffset)
                {
                    request.Headers.IfModifiedSince = dtOffset;
                }
                else
                {
                    throw new InvalidOperationException("Header If-Modified-Since is not DateTime or DateTimeOffset!!");
                }
            }
            else
            {
                var headerValue = ObjectToStringConverter.ToString(valObj);
                request.Headers.Add(_description.Name, headerValue);
            }
        }
    }

    class ContentParameterApplier : IParameterApplier
    {
        readonly ContentRequestParameterDescription _description;
        readonly IApiRequestParameterValueProvider _valueProvider;
        private readonly RequestFactoringSettings _requestFactoringSettings;

        /// <summary>
        /// Initializes a new instance of <see cref="ContentParameterApplier"/>
        /// </summary>
        public ContentParameterApplier(
            ContentRequestParameterDescription description, 
            IApiRequestParameterValueProvider valueProvider,
            RequestFactoringSettings requestFactoringSettings)
        {
            _description = description;
            _valueProvider = valueProvider;
            _requestFactoringSettings = requestFactoringSettings;
        }

        public void Apply(HttpRequestMessage request)
        {
            var valProvider = _valueProvider.GetValue();
            request.Content = _description.ContentFactory.Create(valProvider, _requestFactoringSettings);
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
            var headers = (IEnumerable<KeyValuePair<string, object>>) _valueProvider.GetValue();

            if(headers == null) return;

            foreach (var header in headers)
            {
                var val = ObjectToStringConverter.ToString(header.Value);
                request.Headers.Add(header.Key, val);
            }

        }
    }
}
