using System.Net;
using AutoFixture.Xunit3;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Test;
using TestServer;

namespace E2eTests
{
    public class ApiClientProxyBehavior : IClassFixture<TestApiFixture<Program, ITestServerContract>>
    {
        readonly TestApiFixture<Program, ITestServerContract> _fxt;

        public ApiClientProxyBehavior(TestApiFixture<Program, ITestServerContract> apiFxt, ITestOutputHelper output)
        {
            _fxt = apiFxt;
            _fxt.Output = output;
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.AlreadyReported)]
        // ReSharper disable once InconsistentNaming
        public async Task ShouldPassWhen2xxResponse(HttpStatusCode statusCode)
        {
            //Arrange
            using var asset = _fxt.StartAppWithProxy();
            
            //Act && Assert
            await asset.Proxy.GetStatusAsync(statusCode);
        }

        [Theory]
        [InlineData(HttpStatusCode.Continue)]
        [InlineData(HttpStatusCode.Redirect)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        // ReSharper disable once InconsistentNaming
        public async Task ShouldFailWhenNot2xxResponse(HttpStatusCode statusCode)
        {
            //Arrange
            using var asset = _fxt.StartAppWithProxy();
            
            //Act && Assert
            await Assert.ThrowsAsync<ResponseCodeException>(() => asset.Proxy.GetStatusAsync(statusCode));
        }

        [Theory, AutoData]
        public async Task ShouldTransferJsonPayload(TestDto initial)
        {
            //Arrange
            using var asset = _fxt.StartAppWithProxy();

            //Act
            var actual = await asset.Proxy.GetJsonDto(initial);

            //Assert
            Assert.Equal(initial, actual);
        }

        [Theory, AutoData]
        public async Task ShouldTransferXmlPayload(TestDto initial)
        {
            //Arrange
            using var asset = _fxt.StartAppWithProxy();
            
            //Act
            var actual = await asset.Proxy.GetXmlDto(initial);

            //Assert
            Assert.Equal(initial, actual);
        }
    }
}
