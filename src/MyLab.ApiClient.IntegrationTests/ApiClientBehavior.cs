using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using TestServer;
using TestServer.Controllers;
using Xunit;
using Xunit.Abstractions;
using MyLab.ApiClient.Testing;

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
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);

            //Act
            string res = await client.RootGet();

            //Assert
            Assert.Equal(nameof(IServer.RootGet), res);
        }

        [Fact]
        public async Task ShouldRouteToMethodWithPath()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);

            //Act
            string res = await client.GetWithPath();

            //Assert
            Assert.Equal(nameof(IServer.GetWithPath), res);
        }

        [Fact]
        public async Task ShouldRouteToMethodWithParametrizedPath()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
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
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
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
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
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
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
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
        public async Task ShouldPassAndGetXmlAsBinary()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
            var serializer = new XmlSerializer(typeof(TestObject));
            var obj = new TestObject
            {
                Value = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString()
            };
            byte[] serialized;

            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, obj);
                serialized = mem.ToArray();
            }

            //Act
            var response = await client.PostBinaryXml(serialized);
            
            //Assert

            TestObject gotObj;
            using (var mem = new MemoryStream(response))
            {
                gotObj = (TestObject) serializer.Deserialize(mem);
            }
            
            Assert.Equal(obj.Key, gotObj.Key);
            Assert.Equal(obj.Value, gotObj.Value);
        }
        
        [Fact]
        public async Task ShouldPassAndGetXmlAsString()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
            var serializer = new XmlSerializer(typeof(TestObject));
            var obj = new TestObject
            {
                Value = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString()
            };
            
            var serialized = new StringBuilder();
            using (var writer = new StringWriter(serialized))
            {
                serializer.Serialize(writer, obj);
            }
            
            //Act
            var response = await client.PostStringXml(serialized.ToString());

            //Assert
            TestObject gotObj;
            using (var reader = new StringReader(response))
            {
                gotObj = (TestObject) serializer.Deserialize(reader);
            }
            
            Assert.Equal(obj.Key, gotObj.Key);
            Assert.Equal(obj.Value, gotObj.Value);
        }

        [Fact]
        public async Task ShouldPassAndGetObjectAsJson()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
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

        [Fact]
        public async Task ShouldPassObjectAsForm()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
            var obj = new TestObject
            {
                Value = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString()
            };

            //Act
            var gotObj = await client.PostFormObj(obj);

            //Assert
            Assert.Equal(obj.Key, gotObj.Key);
            Assert.Equal(obj.Value, gotObj.Value);
        }

        [Fact]
        public async Task ShouldSupportDeferredCall()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);
            var obj = new TestObject
            {
                Value = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString()
            };
            var call = client.GetObjUsingCall(obj);

            //Act
            var gotObj = await call.Invoke();

            //Assert
            Assert.Equal(obj.Key, gotObj.Key);
            Assert.Equal(obj.Value, gotObj.Value);
        }

        [Fact]
        public async Task ShouldPassHeaderFromParameter()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);

            //Act
            var header = await client.PostHeaderDirect("foo");

            //Assert
            Assert.Equal("foo", header);
        }

        [Fact]
        public async Task ShouldPassHeaderFromParameterWithRenaming()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);

            //Act
            var header = await client.PostHeaderDirectWithParameterRenaming("foo");

            //Assert
            Assert.Equal("foo", header);
        }

        [Fact]
        public async Task ShouldPassHeaderWithFactory()
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output, 
                Enumerable.Repeat("superheader", 1).ToDictionary(s => s, s => "foo"));

            //Act
            var header = await client.PostHeaderWithFactory();

            //Assert
            Assert.Equal("foo", header);
        }

        [Theory]
        [InlineData(200, "OK")]
        [InlineData(400, "BadRequest")]
        [InlineData(500, "InternalServerError")]
        public async Task ShouldGetCodeResult(int code, string msg)
        {
            //Arrange
            var client = ApiClient<IServer>.Factory.Create(_factory, _output);

            //Act
            var codeResult = await client.GetCode(code, msg);

            //Assert
            Assert.Equal(code, (int)codeResult.StatusCode);
            Assert.Equal(msg, codeResult.Message);
        }
    }
}
