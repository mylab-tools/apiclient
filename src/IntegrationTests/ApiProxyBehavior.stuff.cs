using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class ApiProxyBehavior
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly ITestOutputHelper _output;

        public ApiProxyBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;
        }

        private ITestServer CreateProxy()
        {
            var services = new ServiceCollection();

            try
            {
                services.AddApiClients(
                    registrar => { registrar.RegisterContract<ITestServer>(); },
                    null,
                    new WebApplicationFactoryHttpClientFactory<Startup>(_webApplicationFactory)
                );
            }
            catch (ApiContractValidationException e)
            {
                _output.WriteLine(e.ValidationResult.ToString());
                throw;
            }

            var serviceProvider = services.BuildServiceProvider();
            var api = (ITestServer) serviceProvider.GetService(typeof(ITestServer));
            return api;
        }

        [Fact]
        public async Task ShouldNAME()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            var res = await api.GetExpectedString404();

            //Assert
            Assert.Equal(default, res);
        }
    }
}