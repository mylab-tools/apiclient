using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;

namespace IntegrationTests
{
    public partial class ApiProxyBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        [Fact]
        public async Task ShouldCall()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            var resMsg = await api.CallEcho("foo");

            //Assert
            Assert.Equal("foo", resMsg);
        }

        [Fact]
        public async Task ShouldCallWithoutResponse()
        {
            //Arrange
            var api = CreateProxy();

            //Act & Assert
            await api.CallEchoWithoutResponse("foo");
        }

        [Fact]
        public async Task ShouldCallAndGetDetails()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            CallDetails<string> call = await api.CallEchoAndGetDetails("foo");

            //Assert
            Assert.Equal(HttpStatusCode.OK, call.StatusCode);
            Assert.Equal("foo", call.ResponseContent);
            Assert.Equal(3, call.ResponseMessage.Content.Headers.ContentLength);
        }

        [Fact]
        public async Task ShouldGetBinaryWithDetails()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            CallDetails<byte[]> call = await api.GetBinAndGetDetails();
            var respStr = Encoding.UTF8.GetString(call.ResponseContent);

            //Assert
            Assert.Equal(HttpStatusCode.OK , call.StatusCode);
            Assert.Equal("foo" , respStr);
        }

        [Fact]
        public async Task ShouldGetEnumValue()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            var res = await api.GetEnumVal2();
            
            //Assert
            Assert.Equal(TestEnum.Value2, res);
        }

        [Fact]
        public async Task ShouldGetEnumValueWithDetails()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            var res = await api.GetEnumVal2WithDetails();

            //Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Equal(TestEnum.Value2, res.ResponseContent);
        }

        [Api(Key = "No matter for this test")]
        interface ITestServer
        {
            [Get("echo")]
            Task<string> CallEcho([JsonContent] string msg);

            [Get("echo")]
            Task CallEchoWithoutResponse([JsonContent] string msg);

            [Get("echo")]
            Task<CallDetails<string>> CallEchoAndGetDetails([JsonContent] string msg);

            [Get("resp-content/data/bin-octet-stream")]
            Task<CallDetails<byte[]>> GetBinAndGetDetails();

            [Get("resp-content/data/enum-val-2")]
            Task<TestEnum> GetEnumVal2();

            [Get("resp-content/data/enum-val-2")]
            Task<CallDetails<TestEnum>> GetEnumVal2WithDetails();
        }
    }
}
