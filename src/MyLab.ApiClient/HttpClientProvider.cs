using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Specifies <see cref="HttpClient"/> provider
    /// </summary>
    public interface IHttpClientProvider
    {
        /// <summary>
        /// Provides http client
        /// </summary>
        HttpClient Provide();
    }

    public class FactoryHttpClientProvider : IHttpClientProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _serviceName;

        /// <summary>
        /// Initializes a new instance of <see cref="FactoryHttpClientProvider"/>
        /// </summary>
        public FactoryHttpClientProvider(IHttpClientFactory httpClientFactory)
            :this(httpClientFactory, Options.DefaultName)
        {
            
        }
        /// <summary>
        /// Initializes a new instance of <see cref="FactoryHttpClientProvider"/>
        /// </summary>
        public FactoryHttpClientProvider(IHttpClientFactory httpClientFactory, string serviceName)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _serviceName = serviceName;
        }

        public HttpClient Provide()
        {
            return _httpClientFactory.CreateClient(_serviceName);
        }
    }

    public class SingleHttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of <see cref="SingleHttpClientProvider"/>
        /// </summary>
        public SingleHttpClientProvider(HttpClient client)
        {
            _client = client;
        }

        public HttpClient Provide()
        {
            return _client;
        }
    }

    public class DelegateHttpClientProvider : IHttpClientProvider
    {
        private readonly Func<HttpClient> _factoryMethod;

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateHttpClientProvider"/>
        /// </summary>
        public DelegateHttpClientProvider(Func<HttpClient> factoryMethod)
        {
            _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
        }

        public HttpClient Provide()
        {
            return _factoryMethod();
        }
    }
}