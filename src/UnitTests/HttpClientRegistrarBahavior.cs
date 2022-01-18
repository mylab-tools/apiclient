using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class HttpClientRegistrarBehavior
    {
        [Theory]
        [InlineData("http://localohst/service", "http://localohst/service/")]
        [InlineData("http://localohst/service/", "http://localohst/service/")]
        public void ShouldRegisterUrlWithTRailedSlash(string url, string expected)
        {
            //Arrange
            var srvColl = new ServiceCollection();
            HttpClientRegistrar.Register(srvColl, new ApiClientsOptions
            {
                List =
                {
                    {"key", new ApiConnectionOptions { Url = url }}
                }
            });

            var srvProvider = srvColl.BuildServiceProvider();
            var httpClientFactory = srvProvider.GetService<IHttpClientFactory>();

            //Act
            var httpClient = httpClientFactory.CreateClient("key");

            //Assert
            Assert.Equal(expected, httpClient.BaseAddress.AbsoluteUri);
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
