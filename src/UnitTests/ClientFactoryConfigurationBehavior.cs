using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
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
                registrar =>
                {
                    registrar.RegisterContract<IApiContract>();
                },
                o =>
                {
                    o.List.Add("foo", new ApiConnectionOptions { Url = "http://test.com" });
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

            services.AddApiClients(registrar =>
            {
                registrar.RegisterContract<IApiContract>();
            }, config);

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

        [Api(Key = "foo")]
        interface IApiContract
        {

        }
    }
}
