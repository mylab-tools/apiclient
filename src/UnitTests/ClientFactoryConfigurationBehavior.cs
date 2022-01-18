using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ClientFactoryConfigurationBehavior
    {
        [Fact]
        public void ShouldConfigureFactoryAddress()
        {
            //Arrange
            var services = new ServiceCollection();
            
            services.AddApiClients(
                null,
                new ApiClientsOptions
            {
                List = 
                {
                    { "foo", new ApiConnectionOptions{Url = "http://test.com"}}
                }
            });

            var serviceProvider = services.BuildServiceProvider();
            var srv = ActivatorUtilities.CreateInstance<TestService>(serviceProvider);

            //Act
            var httpClient = srv.CreateHttpClient();

            //Assert
            Assert.NotNull(httpClient);
            Assert.Equal("http://test.com/", httpClient.BaseAddress?.OriginalString);
        }

        [Fact]
        public void ShouldConfigureFactoryFromConfig()
        {
            //Arrange
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("example-config.json");

            var config = configBuilder.Build();

            var services = new ServiceCollection();

            services.AddApiClients(null, config);

            var serviceProvider = services.BuildServiceProvider();
            var srv = ActivatorUtilities.CreateInstance<TestService>(serviceProvider);

            //Act
            var httpClient = srv.CreateHttpClient();

            //Assert
            Assert.NotNull(httpClient);
            Assert.Equal("http://test.com/", httpClient.BaseAddress?.OriginalString);
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
