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
        
        public Dictionary<string, string> Headers => _headers;

        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;

        public string BasePath { get; set; }

        public ApiClientBuilder<TContract> AddHeader(string name, string value)
        {
            _headers.Add(name, value);

            return this;
        }

        public TContract Create()
        {
            var httpInvoker = new HttpRequestInvoker(new HttpOptions
            {
                Headers = new ReadOnlyDictionary<string, string>(_headers),
                Timeout = Timeout,
                BasePath = BasePath
            });

            var clientStrategy = new WebClientProxyStrategy(httpInvoker);
            var description = ApiClientDescription.Get(typeof(TContract));

            return ClientProxy<TContract>.CreateProxy(description, clientStrategy);
        }
    }
}