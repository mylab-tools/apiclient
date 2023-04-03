using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;
using ResponseCodeException = MyLab.ApiClient.ResponseCodeException;

namespace IntegrationTests
{
    public class ApiClientFactoringBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public ApiClientFactoringBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;
        }

        [Fact]
        public async Task ShouldNotFailIfContractHasNoPaths()
        {
            //Arrange
            var clientProvider = new DelegateHttpClientProvider(_webApplicationFactory.CreateClient);
            var client = new ApiClient<ITestContractWithoutPath>(clientProvider);
            HttpStatusCode? actualCode = null;

            //Act
            try
            {
                await client.Request(s => s.Get()).CallAsync();
            }
            catch (ResponseCodeException e)
            {
                actualCode = e.StatusCode;
            }

            //Assert
            Assert.True(actualCode.HasValue);
            Assert.Equal(HttpStatusCode.NotFound, actualCode.Value);
        }

        [Fact]
        public async Task ShouldCreateWithHttpClientFactory()
        {
            //Arrange
            var services = new ServiceCollection()
                .AddApiClients(
                    registrar => { },
                    new WebApplicationFactoryHttpClientFactory<Startup>(_webApplicationFactory)
                )
                .BuildServiceProvider();

            var apiClientFactory = services.GetService<IApiClientFactory>();
            var apiClient = apiClientFactory.CreateApiClient<ITestServer>();

            //Act
            var resp = await apiClient.Request(s => s.Echo("foo")).GetDetailedAsync();

            OutputResponse(resp);

            //Assert
            Assert.Equal("foo", resp.ResponseContent);

        }

        private void OutputResponse(CallDetails<string> resp)
        {
            _output.WriteLine("Resquest dump:");
            _output.WriteLine(resp.RequestDump);
            _output.WriteLine("Response dump:");
            _output.WriteLine(resp.ResponseDump);
        }

        [Api]
        interface ITestContractWithoutPath
        {
            [Get]
            Task Get();
        }

        [Api("echo", Key = "No matter for this test")]
        interface ITestServer
        {
            [Get]
            Task<string> Echo([JsonContent] string msg);
        }
    }
}
