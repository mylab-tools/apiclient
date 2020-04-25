using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests
{
    class WebApplicationFactoryHttpClientFactory<TStartup> : IHttpClientFactory
        where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> _webApplicationFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClientFactoringBasedOnHttpClientFactoryBehavior.WebApplicationFactoryHttpClientFactory{TStartup}"/>
        /// </summary>
        public WebApplicationFactoryHttpClientFactory(WebApplicationFactory<TStartup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }
        public HttpClient CreateClient(string name)
        {
            return _webApplicationFactory.CreateClient();
        }
    }
}
