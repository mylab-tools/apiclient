using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Usage;

namespace MyLab.ApiClient.Tests.Usage;

public partial class ApiClientProxyBehavior
{
    readonly ApiClientOptions _defaultOptions = new();

    HttpResponseMessage CreateOkResponse(string? content = null)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        if (content != null)
        {
            response.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }

        return response;
    }

    Mock<IRequestProcessor> CreateReqProcMock(HttpResponseMessage response)
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
            .Returns<HttpRequestMessage, CancellationToken>((_,_) => Task.FromResult(response));

        return reqProcMock;
    }

    void VerifyResponseUrl(Mock<IRequestProcessor> mock, string expectedUrl)
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

    void VerifyResponseContent(Mock<IRequestProcessor> mock, string expectedContent)
    {
        mock.Verify(req =>
            req.ProcessRequestAsync
            (
                It.Is<HttpRequestMessage>(r => CheckResponseValue(r, expectedContent)),
                It.IsAny<CancellationToken>()
            )
        );
    }

    bool CheckResponseValue(HttpRequestMessage input, string expected)
    {
        var actualContent = input.Content!.ReadAsStringAsync().Result;

        return actualContent == expected;
    }

    interface IContract
    {
        [Post("void")]
        public Task PerformVoidAsync([JsonContent] int parameter);

        [Post("get")]
        public Task<string> GetAsync();

        [Get("newtonjson")]
        public Task<NewtonJsonModel> GetNewtonJsonModel();

        [Get("microsoft")]
        public Task<MicrosoftModel> GetMicrosoftModel();

        [Post("send/{value}")]
        public Task SendInt([Path] int value);

        [Post("send/{value}")]
        public Task SendGuid([Path("value")] Guid id);

        [Post("send-obj")]
        public Task SendObj([JsonContent] NewtonJsonModel obj);
    }

    class NewtonJsonModel
    {
        public const string ValuePropertyName = "nj_val";

        [Newtonsoft.Json.JsonProperty(ValuePropertyName)]
        public string? Value { get; set; }
    }

    class MicrosoftModel
    {
        public const string ValuePropertyName = "ms_val";

        [System.Text.Json.Serialization.JsonPropertyName(ValuePropertyName)]
        public string? Value { get; set; }
    }
}