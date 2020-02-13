using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace MyLab.ApiClient
{
    public interface IHttpClientProvider
    {
        HttpClient Provide();
    }

    class DefaultHttpClientProvider : IHttpClientProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _serviceName;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultHttpClientProvider"/>
        /// </summary>
        public DefaultHttpClientProvider(IHttpClientFactory httpClientFactory)
            :this(httpClientFactory, Options.DefaultName)
        {
            
        }
        /// <summary>
        /// Initializes a new instance of <see cref="DefaultHttpClientProvider"/>
        /// </summary>
        public DefaultHttpClientProvider(IHttpClientFactory httpClientFactory, string serviceName)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _serviceName = serviceName;
        }

        /// <summary>
        /// Provides http client factory
        /// </summary>
        public HttpClient Provide()
        {
            return _httpClientFactory.CreateClient(_serviceName);
        }
    }
}