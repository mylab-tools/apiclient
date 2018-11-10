using System;
using System.Collections.Generic;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Web API client factory
    /// </summary>
    public class ApiClient<TContract>
        where TContract : class 
    {
        /// <summary>
        /// Api client factory
        /// </summary>
        public static readonly ApiClient<TContract> Factory = new ApiClient<TContract>();

        ApiClient()
        {
            
        }

        /// <summary>
        /// Creates web api client 
        /// </summary>
        public TContract Create(IHttpClientProvider httpClientProvider, IHttpMessagesListener httpMessagesListener = null)
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
        /// Creates web api client 
        /// </summary>
        public TContract Create(string baseUrl, TimeSpan? timeout = null, IDictionary<string, string> headers = null)
        {
            var httpClientProvider = new DefaultHttpClientProvider(baseUrl)
            {
                Timeout = timeout.GetValueOrDefault(),
                Headers = headers == null 
                    ? null
                    : new Dictionary<string, string>(headers)

            };

            return Create(httpClientProvider);
        }
    }
}
