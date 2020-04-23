using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace IntegrationTests
{
    public class RespInfoApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ApiClient<ITestServer> _client;

        /// <summary>
        /// Initializes a new instance of <see cref="RespInfoApiClientBehavior"/>
        /// </summary>
        public RespInfoApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;

            var clientProvider = new TestHttpClientProvider(webApplicationFactory);
            _client = ApiClient<ITestServer>.Create(clientProvider);
        }

        [Fact]
        [Category("Response info")]
        public async Task ShouldThrowWhenUnexpectedStatusCode()
        {
            //Arrange

            //Act & Assert
            try
            {
                await _client.Call(s => s.GetUnexpected404()).GetResult();
            }
            catch (ResponseCodeException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                //Pass
            }
        }

        [Fact]
        [Category("Response info")]
        public async Task ShouldPassWhenExpectedStatusCode()
        {
            //Arrange

            //Act & Assert
            await _client.Call(s => s.GetExpected404()).GetResult();
        }

        [Fact]
        [Category("Response info")]
        public async Task ShouldProvideStatusMessage()
        {
            //Arrange

            //Act
            var resp = await _client
                .Call(s => s.GetExpected404())
                .GetResult();

            //Assert
            Assert.Equal("This is a message", resp);
        }

        [Api("resp-info")]
        public interface ITestServer
        {
            [Get("400")]
            void GetUnexpected404();

            [ExpectedCode(HttpStatusCode.BadRequest)]
            [Get("400")]
            void GetExpected404();
        }
    }
}