using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ContractRegistrationBehavior
    {
        [Fact]
        public void ShouldRegisterContractAndBindWithConfiguredFrameworkClientFactory()
        {
            //Arrange
            var services = new ServiceCollection()
                .AddApiClients(
                    registrar => registrar.RegisterContract<IContract>(),
                    o =>
                    {
                        o.List.Add("foo", new ApiConnectionOptions { Url = "http://test.com/" });
                    })
                .BuildServiceProvider();

            var service = services.GetService<IContract>();

            //Act
            var httpClient = GetClient(service);

            //Assert
            Assert.NotNull(httpClient);
            Assert.Equal("http://test.com/", httpClient.BaseAddress?.OriginalString);
        }

        [Fact]
        public void ShouldRegisterScoped()
        {
            //Arrange
            var srvCollection = new ServiceCollection();
            srvCollection.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://test.com/") });
            srvCollection.AddScopedApiClients(registrar => registrar.RegisterContract<IContract2>());

            var services = srvCollection.BuildServiceProvider();

            var service = services.GetService<IContract2>();

            //Act
            var httpClient = GetClient(service);

            //Assert
            Assert.NotNull(httpClient);
            Assert.Equal("http://test.com/", httpClient.BaseAddress?.OriginalString);
        }

        private HttpClient GetClient(IContract service)
        {
            var api = (ApiProxy<IContract>) service;
            var clP = (IHttpClientProvider) typeof(ApiRequestFactory)
                .GetField("_httpClientProvider", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(api.ApiRequestFactory);
            return clP.Provide();
        }

        private HttpClient GetClient(IContract2 service)
        {
            var api = (ApiProxy<IContract2>)service;
            var clP = (IHttpClientProvider)typeof(ApiRequestFactory)
                .GetField("_httpClientProvider", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(api.ApiRequestFactory);
            return clP.Provide();
        }

        [Api("v1", Key = "foo")]
        interface IContract
        {

        }

        [Api("v1")]
        interface IContract2
        {

        }
    }
}
