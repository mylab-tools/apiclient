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
            ResponseCodeException e = await Assert.ThrowsAsync<ResponseCodeException>(() => client.Request(s => s.GetUnexpected400()).GetResultAsync());
            Assert.Equal(HttpStatusCode.BadRequest, e.StatusCode);
        }

        [Fact]
        public async Task ShouldPassWhenExpectedStatusCode()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act & Assert
            await client.Request(s => s.GetExpected404()).CallAsync();
        }

        [Fact]
        public async Task ShouldProvideOriginalServerMessageWhenError()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act
            ResponseCodeException e = await Assert.ThrowsAsync<ResponseCodeException>(() => client.GetUnexpected400("foo"));
            Assert.Equal(HttpStatusCode.BadRequest, e.StatusCode);

            //Assert
            Assert.Equal("foo", e.ServerMessage);
        }

        [Fact]
        public async Task ShouldThrowWhenUnexpectedStatusCodeWithProxy()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act & Assert
            ResponseCodeException e = await  Assert.ThrowsAsync<ResponseCodeException>(() => client.GetUnexpected400());
            Assert.Equal(HttpStatusCode.BadRequest, e.StatusCode);
        }

        [Fact]
        public async Task ShouldPassWhenExpectedStatusCodeWithProxy()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act & Assert
            await client.GetExpected404();
        }

        [Fact]
        public async Task ShouldFailWhenUnexpected200()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act & Assert
            await Assert.ThrowsAsync<ResponseCodeException>(() =>client.GetUnexpected200());
        }

        [Fact]
        public async Task ShouldPassWhenExpected200()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act & Assert
            await client.GetExpected200();
        }

        [Api("resp-info")]
        public interface ITestServer
        {
            [Get("400")]
            Task<TestResponse> GetUnexpected400();

            [Get("400")]
            Task<TestResponse> GetUnexpected400([Query] string msg);

            [ExpectedCode(HttpStatusCode.BadRequest)]
            [Get("400")]
            Task GetExpected404();

            [ExpectedCode(HttpStatusCode.Accepted)]
            [Get("200")]
            Task GetUnexpected200();

            [ExpectedCode(HttpStatusCode.Accepted)]
            [ExpectedCode(HttpStatusCode.OK)]
            [Get("200")]
            Task GetExpected200();
        }

        public class TestResponse
        {

        }
    }
}