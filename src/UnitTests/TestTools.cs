using MyLab.ApiClient.Options;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MyLab.ApiClient.Usage;

namespace MyLab.ApiClient.Tests
{
    static class TestTools
    {
        public static readonly ApiClientOptions DefaultOptions = new();

        public static HttpResponseMessage CreateOkResponse(string? content = null)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            if (content != null)
            {
                response.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            return response;
        }

        public static HttpRequestMessage CreateSimpleRequest(string? content = null)
        {
            var request = new HttpRequestMessage();

            if (content != null)
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            return request;
        }

        public static Mock<IRequestProcessor> CreateReqProcMock(HttpResponseMessage response)
        {
            var reqProcMock = new Mock<IRequestProcessor>();
            reqProcMock.Setup
                (p =>
                    p.ProcessRequestAsync
                    (
                        It.IsAny<HttpRequestMessage>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns<HttpRequestMessage, CancellationToken>((_, _) => Task.FromResult(response));

            return reqProcMock;
        }

        public static void VerifyRequestUrl(Mock<IRequestProcessor> mock, string expectedUrl)
        {
            mock.Verify
            (req =>
                req.ProcessRequestAsync
                (
                    It.Is<HttpRequestMessage>(r => r.RequestUri!.ToString() == expectedUrl),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        public static void VerifyRequestContent(Mock<IRequestProcessor> mock, string expectedContent)
        {
            mock.Verify(req =>
                req.ProcessRequestAsync
                (
                    It.Is<HttpRequestMessage>(r => CheckResponseValue(r, expectedContent)),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        static bool CheckResponseValue(HttpRequestMessage input, string expected)
        {
            var actualContent = input.Content!.ReadAsStringAsync().Result;

            return actualContent == expected;
        }
    }
}
