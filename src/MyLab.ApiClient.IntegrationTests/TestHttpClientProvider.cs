using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MyLab.ApiClient.IntegrationTests
{
    class TestHttpClientProvider<T> : IHttpClientProvider
        where T : class
    {
        private readonly WebApplicationFactory<T> _factory;

        public TestHttpClientProvider(WebApplicationFactory<T> factory)
        {
            _factory = factory;
        }

        public HttpClient Provide()
        {
            return _factory.CreateClient();
        }
    }
}