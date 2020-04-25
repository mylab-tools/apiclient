using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;

namespace IntegrationTests
{
    class TestHttpClientProvider<TStartup> : IHttpClientProvider
        where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> _applicationFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="TestHttpClientProvider{T}"/>
        /// </summary>
        public TestHttpClientProvider(WebApplicationFactory<TStartup> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }

        public HttpClient Provide()
        {
            return _applicationFactory.CreateClient();
        }
    }
}