using System.Net;
using AutoFixture.Xunit3;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Test;
using TestServer;

namespace E2eTests
{
    public class ApiClientProxyBehavior : IClassFixture<TestApiFixture<Program, ITestServerContract>>
    {
        readonly ITestServerContract _proxy;

        public ApiClientProxyBehavior(TestApiFixture<Program, ITestServerContract> apiFxt, ITestOutputHelper output)
        {
            apiFxt.Output = output;
            _proxy = apiFxt.StartAppWithProxy().Proxy;
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.AlreadyReported)]
        // ReSharper disable once InconsistentNaming
        public async Task ShouldPassWhen2xxResponse(HttpStatusCode statusCode)
        {
            //Act && Assert
            await _proxy.GetStatusAsync(statusCode);
        }

        [Theory]
        [InlineData(HttpStatusCode.Continue)]
        [InlineData(HttpStatusCode.Redirect)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        // ReSharper disable once InconsistentNaming
        public async Task ShouldFailWhenNot2xxResponse(HttpStatusCode statusCode)
        {
            //Act && Assert
            await Assert.ThrowsAsync<ResponseCodeException>(() => _proxy.GetStatusAsync(statusCode));
        }

        [Theory, AutoData]
        public async Task ShouldTransferJsonPayload(TestDto initial)
        {
            //Arrange

            //Act
            var actual = await _proxy.GetJsonDto(initial);

            //Assert
            Assert.Equal(initial, actual);
        }

        [Theory, AutoData]
        public async Task ShouldTransferXmlPayload(TestDto initial)
        {
            //Arrange

            //Act
            var actual = await _proxy.GetXmlDto(initial);

            //Assert
            Assert.Equal(initial, actual);
        }
    }
}
