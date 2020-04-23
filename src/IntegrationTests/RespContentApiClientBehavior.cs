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
        private readonly ApiClient<ITestServer> _client;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public RespContentApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;

            var clientProvider = new TestHttpClientProvider(webApplicationFactory);
            _client = ApiClient<ITestServer>.Create(clientProvider);
        }

        [Fact]
        [Category("Response content")]
        public async Task ShouldProvideXmlResponse()
        {
            //Arrange


            //Act
            var resp = await _client
                .Call(s => s.GetXmlObj())
                .GetResult();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        [Category("Response content")]
        public async Task ShouldProvideJsonResponse()
        {
            //Arrange


            //Act
            var resp = await _client
                .Call(s => s.GetJsonObj())
                .GetResult();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Api("resp-content")]
        public interface ITestServer
        {

            [Get("data/xml")]
            TestModel GetXmlObj();

            [Get("data/json")]
            TestModel GetJsonObj();
        }
    }
}