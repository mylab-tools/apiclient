using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class DiIntegrationBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly ITestOutputHelper _output;

        public DiIntegrationBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;
        }

        [Fact]
        public async Task ShouldIntegrateProxy()
        {
            //Arrange
            var services = new ServiceCollection();

            try
            {
                services.AddApiClients(
                    registrar =>
                    {
                        registrar.RegisterContract<ITestServer>();
                    },
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
            var srv = ActivatorUtilities.CreateInstance<TestServiceForProxy>(serviceProvider);

            //Act
            var resMsg = await srv.TestMethod("foo");

            //Assert
            Assert.NotNull(resMsg);
            Assert.Equal("foo", resMsg);

        }

        [Api("echo", Key = "No matter for this test")]
        interface ITestServer
        {
            [Get]
            Task<string> Echo([JsonContent]string msg);
        }

        class TestServiceForProxy
        {
            private readonly ITestServer _server;

            public TestServiceForProxy(ITestServer server)
            {
                _server = server;
            }

            public Task<string> TestMethod(string msg)
            {
                return _server.Echo(msg);
            }
        }
    }
}
