using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
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
        public async Task ShouldCallAndGetDetails()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            CallDetails<string> call = await api.CallEchoAndGetDetails("foo");

            //Assert
            Assert.Equal("foo", call.ResponseContent);
            Assert.Equal(HttpStatusCode.OK, call.StatusCode);
            Assert.Equal(3, call.ResponseMessage.Content.Headers.ContentLength);
        }

        [Api("echo", Key = "No matter for this test")]
        interface ITestServer
        {
            [Get]
            Task<string> CallEcho([JsonContent] string msg);

            [Get]
            Task<CallDetails<string>> CallEchoAndGetDetails([JsonContent] string msg);
        }
    }
}
