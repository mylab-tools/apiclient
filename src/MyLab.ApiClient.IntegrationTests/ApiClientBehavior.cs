using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using TestServer;
using TestServer.Controllers;
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

        [Fact]
        public async Task ShouldPassQueryParameter()
        {
            //Arrange
            var client = CreateClient();
            var id = Guid.NewGuid().ToString();

            //Act
            string res = await client.GetWithQuery(id);

            //Assert
            Assert.Equal(id, res);
        }

        [Fact]
        public async Task ShouldPassAndGetBinaryData()
        {
            //Arrange
            var client = CreateClient();
            var value = Guid.NewGuid().ToString();
            var bin = Encoding.UTF8.GetBytes(value);

            //Act
            var gotBin = await client.PostBinary(bin);
            var gotValue = Encoding.UTF8.GetString(gotBin);

            //Assert
            Assert.Equal(value, gotValue);
        }

        [Fact]
        public async Task ShouldPassAndGetObjectAsXml()
        {
            //Arrange
            var client = CreateClient();
            var obj = new TestObject
            {
                Value = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString()
            };
            
            //Act
            var gotObj = await client.PostXmlObj(obj);

            //Assert
            Assert.Equal(obj.Key, gotObj.Key);
            Assert.Equal(obj.Value, gotObj.Value);
        }

        [Fact]
        public async Task ShouldPassAndGetObjectAsJson()
        {
            //Arrange
            var client = CreateClient();
            var obj = new TestObject
            {
                Value = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString()
            };

            //Act
            var gotObj = await client.PostJsonObj(obj);

            //Assert
            Assert.Equal(obj.Key, gotObj.Key);
            Assert.Equal(obj.Value, gotObj.Value);
        }

        IServer CreateClient()
        {
            var b = new ApiClientBuilder<IServer>();
            return b.Create(
                new TestHttpClientProvider<Startup>(_factory), 
                new TestConsoleWriterHttpMessageListener(_output));
        }
    }

    internal class TestConsoleWriterHttpMessageListener : IHttpMessagesListener
    {
        private readonly ITestOutputHelper _output;

        public TestConsoleWriterHttpMessageListener(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Notify(HttpRequestMessage request, HttpResponseMessage response)
        {
            _output.WriteLine("========================================");
            _output.WriteLine("HTTP Request:");
            _output.WriteLine("========================================");
            _output.WriteLine(request.Method + " " + request.RequestUri);
            foreach (var header in request.Headers)
                _output.WriteLine(header.Key + ": " + string.Join(", ", header.Value));
            _output.WriteLine("");
            try
            {
                _output.WriteLine(request.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                _output.WriteLine("Error: " + e.Message);
            }
            _output.WriteLine("========================================");
            _output.WriteLine("HTTP Response:");
            _output.WriteLine("========================================");
            _output.WriteLine((int)response.StatusCode + " " + response.StatusCode);
            foreach (var header in response.Headers)
                _output.WriteLine(header.Key + ": " + header.Value);
            _output.WriteLine("");
            try
            {
                _output.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                _output.WriteLine("Error: " + e.Message);
            }
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

        [ApiGet(RelPath = "foo/bar/q")]
        Task<string> GetWithQuery([QueryParam] string id);

        [ApiPost(RelPath = "post/bin")]
        Task<byte[]> PostBinary([BinaryBody]byte[] bin);

        [ApiPost(RelPath = "post/xml-object")]
        Task<TestObject> PostXmlObj([XmlBody]TestObject testObject);

        [ApiPost(RelPath = "post/json-object")]
        Task<TestObject> PostJsonObj([JsonBody]TestObject testObject);
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
