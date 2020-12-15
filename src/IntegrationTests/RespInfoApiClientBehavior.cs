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
        private readonly IHttpClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="RespInfoApiClientBehavior"/>
        /// </summary>
        public RespInfoApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;

            _clientProvider = new DelegateHttpClientProvider(webApplicationFactory.CreateClient);
        }

        [Fact]
        public async Task ShouldThrowWhenUnexpectedStatusCode()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act & Assert
            try
            {
                await client.Method(s => s.GetUnexpected404()).CallAsync();
            }
            catch (ResponseCodeException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                //Pass
            }
        }

        [Fact]
        public async Task ShouldPassWhenExpectedStatusCode()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act & Assert
            await client.Method(s => s.GetExpected404()).CallAsync();
        }

        [Fact]
        public async Task ShouldNotProvideStatusMessageIfNotSuccess()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act & Assert
            await client
                .Method(s => s.GetExpected404())
                .CallAsync();
        }

        [Fact]
        public async Task ShouldThrowWhenUnexpectedStatusCodeWithProxy()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act & Assert
            try
            {
                await client.GetUnexpected404();
            }
            catch (ResponseCodeException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                //Pass
            }
        }

        [Fact]
        public async Task ShouldPassWhenExpectedStatusCodeWithProxy()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act & Assert
            await client.GetExpected404();
        }

        [Api("resp-info")]
        public interface ITestServer
        {
            [Get("400")]
            Task GetUnexpected404();

            [ExpectedCode(HttpStatusCode.BadRequest)]
            [Get("400")]
            Task GetExpected404();
        }
    }
}