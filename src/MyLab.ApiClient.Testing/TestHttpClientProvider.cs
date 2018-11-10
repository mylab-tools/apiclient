using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MyLab.ApiClient.Testing
{
    class TestHttpClientProvider<T> : IHttpClientProvider
        where T : class
    {
        private readonly WebApplicationFactory<T> _factory;

        public Dictionary<string, string> Headers { get; set; }

        public TestHttpClientProvider(WebApplicationFactory<T> factory)
        {
            _factory = factory;
        }

        public HttpClient Provide()
        {
            var c = _factory.CreateClient();

            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    c.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            return c;
        }
    }
}