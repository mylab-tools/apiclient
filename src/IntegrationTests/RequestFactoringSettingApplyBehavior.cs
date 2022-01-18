using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class RequestFactoringSettingApplyBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly ITestOutputHelper _output;

        public RequestFactoringSettingApplyBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;
        }

        private ITestServer CreateProxy(Action<ApiClientsOptions> configure)
        {
            var services = new ServiceCollection();

            try
            {
                services.AddApiClients(
                    registrar => { registrar.RegisterContract<ITestServer>(); },
                    configure,
                    new WebApplicationFactoryHttpClientFactory<Startup>(_webApplicationFactory)
                );
            }
            catch (ApiContractValidationException e)
            {
                _output.WriteLine(e.ValidationResult.ToString());
                throw;
            }

            var serviceProvider = services.BuildServiceProvider();
            var api = (ITestServer)serviceProvider.GetService(typeof(ITestServer));
            return api;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldApplyIgnoreNullFields(bool ignore)
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = null
            };

            var proxy = CreateProxy(o => o.JsonSettings.IgnoreNullValues = ignore);

            //Act
            var resp = await proxy.CallEchoObj(model);
            
            //Assert
            if(ignore)
                Assert.DoesNotContain("\"TestValue\":null", resp);
            else
                Assert.Contains("\"TestValue\":null", resp);
        }

        [Fact]
        public async Task ShouldIgnoreNullFieldsByDefault()
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = null
            };

            var proxy = CreateProxy(o =>{});

            //Act
            var resp = await proxy.CallEchoObj(model);

            //Assert
            Assert.DoesNotContain("\"TestValue\":null", resp);
        }

        [Api(Key = "No matter for this test")]
        interface ITestServer
        {
            [Get("echo/obj")]
            Task<string> CallEchoObj([JsonContent] TestModel msg);
        }
    }
}
