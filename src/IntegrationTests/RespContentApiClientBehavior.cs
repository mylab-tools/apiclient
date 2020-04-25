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
    public class RespContentApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly TestHttpClientProvider<Startup> _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public RespContentApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;
           _clientProvider = new TestHttpClientProvider<Startup>(webApplicationFactory);
        }

        [Fact]
        public async Task ShouldProvideXmlResponse()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act
            var resp = await client
                .Call(s => s.GetXmlObj())
                .GetResult();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponse()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act
            var resp = await client
                .Call(s => s.GetJsonObj())
                .GetResult();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideXmlResponseWithProxy()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act
            var resp = await client.GetXmlObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponseWithProxy()
        {
            //Arrange
            var client = ApiProxy<ITestServer>.Create(_clientProvider);

            //Act
            var resp = await client.GetJsonObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Api("resp-content")]
        public interface ITestServer
        {

            [Get("data/xml")]
            Task<TestModel> GetXmlObj();

            [Get("data/json")]
            Task<TestModel> GetJsonObj();
        }
    }
}