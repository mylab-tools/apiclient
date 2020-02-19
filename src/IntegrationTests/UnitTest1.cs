using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class ApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly ITestOutputHelper _output;
        private ApiClient<ITestServer> _client;

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClientBehavior"/>
        /// </summary>
        public ApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;

            var clientProvider = new TestHttpClientProvider(_webApplicationFactory);
            _client = ApiClient<ITestServer>.Create(clientProvider);
        }

        [Fact]
        public async Task ShouldThrowWhenUnexpectedStatusCode()
        {
            //Arrange

            //Act & Assert
            await Assert.ThrowsAsync<ResponseCodeException>(() =>
            {
                return _client.Request(server => server.GetUnexpected404()).Send();
            });
        }

        [Fact]
        public async Task ShouldPassWhenExpectedStatusCode()
        {
            //Arrange

            //Act & Assert
            await _client.Request(server => server.GetExpected404()).Send();
        }

        [Fact]
        public async Task ShouldProvideStatusMessage()
        {
            //Arrange

            //Act
            var resp = await _client.Request(server => server.GetExpected404()).Send();

            //Assert
            Assert.Equal("This is a message", resp);
        }
    }

    [Api("test")]
    interface ITestServer
    {
        [Get("400")]
        void GetUnexpected404();

        [ExpectedCode(HttpStatusCode.BadRequest)]
        [Get("400")]
        void GetExpected404();
    }
}
