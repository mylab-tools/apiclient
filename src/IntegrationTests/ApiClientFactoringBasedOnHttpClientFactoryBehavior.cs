using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class ApiClientFactoringBasedOnHttpClientFactoryBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public ApiClientFactoringBasedOnHttpClientFactoryBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;
        }

        [Fact]
        public async Task ShouldCreateValidClient()
        {
            //Arrange
            var services = new ServiceCollection();

            services.AddApiClients(
                null,
                new WebApplicationFactoryHttpClientFactory<Startup>(_webApplicationFactory)
            );

            var serviceProvider = services.BuildServiceProvider();
            TestServiceForHttpClientFactory srv;

            try
            {
                srv = ActivatorUtilities.CreateInstance<TestServiceForHttpClientFactory>(serviceProvider);
            }
            catch (ApiContractValidationException e)
            {
                _output.WriteLine(e.ValidationResult.ToString());
                throw;
            }

            //Act
            var resMsg = await srv.TestMethod("foo", _output);

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

        class TestServiceForHttpClientFactory
        {
            private readonly ApiClient<ITestServer> _server;

            public TestServiceForHttpClientFactory(IHttpClientFactory httpClientFactory)
            {
                _server = httpClientFactory.CreateApiClient<ITestServer>();
            }

            public async Task<string> TestMethod(string msg, ITestOutputHelper log)
            {
                var resp = await _server.Request(s => s.Echo(msg)).GetDetailedAsync();

                log.WriteLine("Resquest dump:");
                log.WriteLine(resp.RequestDump);
                log.WriteLine("Response dump:");
                log.WriteLine(resp.ResponseDump);

                return resp.ResponseContent;
            }
        }
    }
}
