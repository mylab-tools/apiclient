using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class ApiRequestFactoryBehavior
    {
        private readonly ITestOutputHelper _output;

        public ApiRequestFactoryBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetUrlCalcCases))]
        public async Task ShouldCreateRightUrl(Type contractType, string methodName, string basePath, string expectedUrl)
        {
            //Arrange
            var msgHandler = new TestMsgHandler();
            var httpClient = new HttpClient(msgHandler)
            {
                BaseAddress = new Uri(basePath)
            };

            var httpClientFactory = new SingleHttpClientProvider(httpClient);
            var factory = new ApiRequestFactory(contractType, httpClientFactory);

            _output.WriteLine($"Service URL: {factory.ServiceDescription.Url}");
            _output.WriteLine($"Post method URL: {factory.ServiceDescription.Methods.Values.First().Url}");

            var method = contractType.GetMethod(methodName);

            var request = factory.Create(method, new object[] { "foo" });
            //_output.WriteLine($"Request URL: {request.}");

            //Act
            await request.CallAsync();

            //Assert
            _output.WriteLine($"Actual URL: {msgHandler.LastRequest.RequestUri.AbsoluteUri}");

            Assert.Equal(expectedUrl, msgHandler.LastRequest.RequestUri.AbsoluteUri);
        }

        public static IEnumerable<object[]> GetUrlCalcCases()
        {
            var baseAddresses = new[]
            {
                "http://localhost/service/"
            };
            var list = new List<object[]>();

            foreach (var baseAddress in baseAddresses)
            {
                list.Add(new object[] { typeof(IContract1), nameof(IContract1.Post), baseAddress, baseAddress.TrimEnd('/') + "/foo" });
                list.Add(new object[] { typeof(IContract2), nameof(IContract1.Post), baseAddress, baseAddress.TrimEnd('/') + "/foo" });
                list.Add(new object[] { typeof(IContract3), nameof(IContract2.Post), baseAddress, baseAddress.TrimEnd('/') + "/foo" });
                list.Add(new object[] { typeof(IContract4), nameof(IContract3.Post), baseAddress, baseAddress.TrimEnd('/') + "/foo/bar" });
            }

            return list;
        }

        [Api("")]
        public interface IContract1
        {
            [Post("foo")]
            Task<string> Post([JsonContent] string request);
        }

        [Api]
        public interface IContract2
        {
            [Post("foo")]
            Task<string> Post([JsonContent] string request);
        }

        [Api("foo")]
        public interface IContract3
        {
            [Post("")]
            Task<string> Post([JsonContent] string request);
        }

        [Api("foo")]
        public interface IContract4
        {
            [Post("bar")]
            Task<string> Post([JsonContent] string request);
        }

        class TestMsgHandler : HttpMessageHandler
        {
            public HttpRequestMessage LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}
