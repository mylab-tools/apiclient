using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Web API client builder
    /// </summary>
    public class ApiClientBuilder<TContract> 
        where TContract : class
    {
        readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        
        /// <summary>
        /// Default headers for all requests
        /// </summary>
        public Dictionary<string, string> Headers => _headers;

        /// <summary>
        /// Connection timeout
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Base service URL
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Adds default header for all requests
        /// </summary>
        public ApiClientBuilder<TContract> AddHeader(string name, string value)
        {
            _headers.Add(name, value);

            return this;
        }

        internal TContract Create(IHttpClientProvider httpClientProvider, IHttpMessagesListener httpMessagesListener)
        {
            var httpInvoker = new HttpRequestInvoker(httpClientProvider);

            var clientStrategy = new WebClientProxyStrategy(httpInvoker)
            {
                HttpMessagesListener = httpMessagesListener
            };
            var description = ApiClientDescription.Get(typeof(TContract));

            return ClientProxy<TContract>.CreateProxy(description, clientStrategy);
        }

        /// <summary>
        /// Creates a client
        /// </summary>
        public TContract Create()
        {
            var httpClientProvider = new DefaultHttpClientProvider(BaseUrl)
            {
                Timeout = Timeout,
                Headers = _headers
            };

            return Create(httpClientProvider, null);
        }
    }
}