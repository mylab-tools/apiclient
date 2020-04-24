using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using Xunit;

namespace IntegrationTests
{
    public class ClientFactoryConfigurationBehavior
    {
        [Fact]
        public void ShouldConfigureFactoryAddress()
        {
            //Arrange
            var services = new ServiceCollection();
            
            //services.AddApiClients();

            var serviceProvider = services.BuildServiceProvider();
            var srv = ActivatorUtilities.CreateInstance<TestService>(serviceProvider);

            //Act
            var httpClient = srv.CreateHttpClient();

            //Assert
            Assert.NotNull(httpClient);
            Assert.Equal("http://test.com", httpClient.BaseAddress.OriginalString);
        }

        class TestService
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public TestService(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public HttpClient CreateHttpClient()
            {
                return _httpClientFactory.CreateClient("foo");
            }
        }
    }
}