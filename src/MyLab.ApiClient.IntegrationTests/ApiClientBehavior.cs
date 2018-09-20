using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.ApiClient.IntegrationTests
{
    public class ApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public ApiClientBehavior(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Fact]
        public async Task ShouldRouteToRootMethod()
        {
            //Arrange
            var client = CreateClient();

            //Act
            string res = await client.RootGet();

            //Assert
            Assert.Equal(nameof(IServer.RootGet), res);
        }

        [Fact]
        public async Task ShouldRouteToMethodWithPath()
        {
            //Arrange
            var client = CreateClient();

            //Act
            string res = await client.GetWithPath();

            //Assert
            Assert.Equal(nameof(IServer.GetWithPath), res);
        }

        [Fact]
        public async Task ShouldRouteToMethodWithParametrizedPath()
        {
            //Arrange
            var client = CreateClient();
            var id = Guid.NewGuid().ToString();

            //Act
            string res = await client.GetWithParametrizedPath(id);

            //Assert
            Assert.Equal(id, res);
        }

        IServer CreateClient()
        {
            var b = new ApiClientBuilder<IServer>();
            return b.Create(new TestHttpClientProvider<Startup>(_factory));
        }
    }

    [Api("api/test")]
    public interface IServer
    {
        [ApiGet]
        Task<string> RootGet();

        [ApiGet(RelPath = "foo/bar")]
        Task<string> GetWithPath();

        [ApiGet(RelPath = "foo/bar/{id}")]
        Task<string> GetWithParametrizedPath([PathParam] string id);
    }

    class TestHttpClientProvider<T> : IHttpClientProvider
        where T : class
    {
        private readonly WebApplicationFactory<T> _factory;

        public TestHttpClientProvider(WebApplicationFactory<T> factory)
        {
            _factory = factory;
        }

        public HttpClient Provide()
        {
            return _factory.CreateClient();
        }
    }
}
