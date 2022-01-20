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
        public async Task ShouldApplyIgnoreJsonNullFields(bool ignore)
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = null
            };

            var proxy = CreateProxy(o => o.JsonSettings.IgnoreNullValues = ignore);

            //Act
            var resp = await proxy.CallEchoJson(model);
            
            //Assert
            if(ignore)
                Assert.DoesNotContain("\"TestValue\":null", resp);
            else
                Assert.Contains("\"TestValue\":null", resp);
        }

        [Fact]
        public async Task ShouldIgnoreJsonNullFieldsByDefault()
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = null
            };

            var proxy = CreateProxy(o =>{});

            //Act
            var resp = await proxy.CallEchoJson(model);

            //Assert
            Assert.DoesNotContain("\"TestValue\":null", resp);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldApplyIgnoreUrlFormNullFields(bool ignore)
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = null
            };

            var proxy = CreateProxy(o => o.UrlFormSettings.IgnoreNullValues = ignore);

            //Act
            var resp = await proxy.CallEchoForm(model);

            //Assert
            Assert.Equal(ignore ? null : "TestValue=", resp);
        }

        [Fact]
        public async Task ShouldIgnoreUrlFormNullFieldsByDefault()
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = null
            };

            var proxy = CreateProxy(o => { });

            //Act
            var resp = await proxy.CallEchoForm(model);

            //Assert
            Assert.Equal("TestValue=", resp);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldApplyFormEscape(bool escape)
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = "http://test.com/"
            };

            var proxy = CreateProxy(o => o.UrlFormSettings.EscapeSymbols = escape);

            //Act
            var resp = await proxy.CallEchoForm(model);

            //Assert
            Assert.Contains(escape ? "http%3A%2F%2Ftest.com%2F" : "http://test.com/", resp);
        }

        [Fact]
        public async Task ShouldEscapeFormByDefault()
        {
            //Arrange
            var model = new TestModel
            {
                TestValue = "http://test.com/"
            };

            var proxy = CreateProxy(o => { });

            //Act
            var resp = await proxy.CallEchoForm(model);

            //Assert
            Assert.Contains("http%3A%2F%2Ftest.com%2F", resp);
        }

        [Api(Key = "No matter for this test")]
        interface ITestServer
        {
            [Get("echo/body")]
            Task<string> CallEchoJson([JsonContent] TestModel msg);

            [Get("echo/body")]
            Task<string> CallEchoForm([FormContent] TestModel msg);
        }
    }
}
