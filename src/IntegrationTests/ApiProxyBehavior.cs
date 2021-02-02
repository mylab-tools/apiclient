using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;

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
            CallDetails<TestEnum> res = null;

            //Act
            try
            {
                res = await api.GetEnumVal2WithDetails();
            }
            catch (DetailedResponseProcessingException<CallDetails<TestEnum>> e)
            {
                _output.WriteLine("RequestDump:");
                _output.WriteLine(e.CallDetails.RequestDump);
                _output.WriteLine("ResponseDump:");
                _output.WriteLine(e.CallDetails.ResponseDump);
            }

            if (res != null)
            {
                _output.WriteLine("RequestDump:");
                _output.WriteLine(res.RequestDump);
                _output.WriteLine("ResponseDump:");
                _output.WriteLine(res.ResponseDump);
            }


            //Assert
            Assert.NotNull(res);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Equal(TestEnum.Value2, res.ResponseContent);
        }

        [Fact]
        public async Task ShouldGetErrorWhenUnexpected404()
        {
            //Arrange
            var api = CreateProxy();

            //Act && Assert
            await Assert.ThrowsAsync<ResponseCodeException>(() => api.GetUnexpected404());
        }

        [Fact]
        public async Task ShouldGetDefaultValueWhenExpected404()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            var res = await api.GetExpectedInt404();

            //Assert
            Assert.Equal(default, res);
        }

        [Fact]
        public async Task ShouldGetDefaultObjectWhenExpected404()
        {
            //Arrange
            var api = CreateProxy();

            //Act
            var res = await api.GetExpectedString404();

            //Assert
            Assert.Equal(default, res);
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

            [Get("resp-info/404")]
            Task<string> GetUnexpected404();

            [Get("resp-info/404")]
            [ExpectedCode(HttpStatusCode.NotFound)]
            Task<string> GetExpectedString404();

            [Get("resp-info/404")]
            [ExpectedCode(HttpStatusCode.NotFound)]
            Task<int> GetExpectedInt404();
        }
    }
}
