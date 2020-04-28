using System.Collections.Generic;
using System.Linq;
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
        private ApiClient<ITestServer> _client;
        private ITestServer _proxy;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public RespContentApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;
           var clientProvider = new DelegateHttpClientProvider(webApplicationFactory.CreateClient);
           _client = new ApiClient<ITestServer>(clientProvider);
           _proxy = ApiProxy<ITestServer>.Create(clientProvider);
        }

        [Fact]
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

        [Fact]
        public async Task ShouldProvideEnumerableResponse()
        {
            //Arrange
            
            //Act
            var resp = await _client
                .Call(s => s.GetEnumerable())
                .GetResult();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideArrayResponse()
        {
            //Arrange

            //Act
            var resp = await _client
                .Call(s => s.GetArray())
                .GetResult();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideXmlResponseWithProxy()
        {
            //Arrange
            
            //Act
            var resp = await _proxy.GetXmlObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponseWithProxy()
        {
            //Arrange
            
            //Act
            var resp = await _proxy.GetJsonObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideEnumerableResponseWithProxy()
        {
            //Arrange

            //Act
            var resp = await _proxy.GetEnumerable();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideArrayResponseWithProxy()
        {
            //Arrange

            //Act
            var resp = await _proxy.GetArray();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Api("resp-content")]
        public interface ITestServer
        {

            [Get("data/xml")]
            Task<TestModel> GetXmlObj();

            [Get("data/json")]
            Task<TestModel> GetJsonObj();

            [Get("data/enumerable")]
            Task<IEnumerable<string>> GetEnumerable();

            [Get("data/enumerable")]
            Task<string[]> GetArray();
        }
    }
}