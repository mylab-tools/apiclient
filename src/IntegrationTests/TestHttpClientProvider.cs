using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;

namespace IntegrationTests
{
    class TestHttpClientProvider : IHttpClientProvider
    {
        private readonly WebApplicationFactory<Startup> _applicationFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="TestHttpClientProvider"/>
        /// </summary>
        public TestHttpClientProvider(WebApplicationFactory<Startup> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }

        public HttpClient Provide()
        {
            return _applicationFactory.CreateClient();
        }
    }
}