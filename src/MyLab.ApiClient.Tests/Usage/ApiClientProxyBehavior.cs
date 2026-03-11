using JetBrains.Annotations;
using Moq;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Usage;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyLab.ApiClient.JsonSerialization;
using Xunit;

namespace MyLab.ApiClient.Tests.Usage;

[TestSubject(typeof(ApiClientProxy))]
public class ApiClientProxyBehavior
{
    [Fact]
    public async Task ShouldPerformVoidMethod()
    {
        //Arrange
        var opt = new ApiClientOptions();

        var response = new HttpResponseMessage();
        
        var reqProcMock = new Mock<IRequestProcessor>();
        reqProcMock.Setup(p => p.ProcessRequestAsync(It.IsAny<HttpRequestMessage>()))
            .Returns<HttpRequestMessage>(_ => Task.FromResult(response));

        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, opt);

        //Act
        await proxy.PerformVoidAsync(1);

        //Assert
        reqProcMock.Verify(
            req => 
                req.ProcessRequestAsync(
                    It.Is<HttpRequestMessage>(
                        r => r.RequestUri.ToString() == "void"
                    )
                )
            );
    }

    [Fact]
    public async Task ShouldPerformWithReturnParameters()
    {
        //Arrange
        var opt = new ApiClientOptions();

        var reqProcMock = new Mock<IRequestProcessor>();
        reqProcMock.Setup(p => p.ProcessRequestAsync(It.IsAny<HttpRequestMessage>()))
            .Returns<HttpRequestMessage>(req =>
            {
                var reqDigit = int.Parse(req.Content.ReadAsStringAsync().Result);
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent((reqDigit + 1).ToString())
                };

                return Task.FromResult(resp);
            });

        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, opt);

        //Act
        var result = await proxy.PerformAddAsync(5);

        //Assert
        reqProcMock.Verify(
            req => req.ProcessRequestAsync(
                It.Is<HttpRequestMessage>(
                    r => r.RequestUri.ToString() == "add"
                )
            )
        );
        
        Assert.Equal(6, result);
    }

    [Fact]
    public async Task ShouldFailIfStatusCodeIsnNotOK()
    {
        //Arrange
        var opt = new ApiClientOptions();

        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        var reqProcMock = new Mock<IRequestProcessor>();
        reqProcMock.Setup(p => p.ProcessRequestAsync(It.IsAny<HttpRequestMessage>()))
            .Returns<HttpRequestMessage>(_ => Task.FromResult(response));

        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, opt);

        //Act & Assert
        var e = await Assert.ThrowsAsync<ResponseCodeException>(() => proxy.PerformVoidAsync(1));
        Assert.Equal(HttpStatusCode.NotFound, e.StatusCode);
    }

    [Fact]
    public async Task ShouldSupportNewtonJsonModels()
    {
        //Arrange
        var opt = new ApiClientOptions();
        var serializedModel = $"{{\"{NewtonJsonModel.ValuePropertyName}\":\"foo\"}}";

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(serializedModel, Encoding.UTF8, "application/json")
        };

        var reqProcMock = new Mock<IRequestProcessor>();
        reqProcMock.Setup
        (p => 
                p.ProcessRequestAsync(It.IsAny<HttpRequestMessage>())
        ).Returns<HttpRequestMessage>(_ => Task.FromResult(response));

        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, opt);

        //Act
        var actualModel = await proxy.GetNewtonJsonModel();

        //Assert
        Assert.Equal("foo", actualModel.Value);
    }

    [Fact]
    public async Task ShouldSupportMicrosoftModels()
    {
        //Arrange
        var opt = new ApiClientOptions
        {
            JsonSerializer = new MicrosoftJsonSerializer(new ApiJsonSettings())
        };
        var serializedModel = $"{{\"{MicrosoftModel.ValuePropertyName}\":\"bar\"}}";

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(serializedModel, Encoding.UTF8, "application/json")
        };

        var reqProcMock = new Mock<IRequestProcessor>();
        reqProcMock.Setup
        (p =>
            p.ProcessRequestAsync(It.IsAny<HttpRequestMessage>())
        ).Returns<HttpRequestMessage>(_ => Task.FromResult(response));

        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, opt);

        //Act
        var actualModel = await proxy.GetMicrosoftModel();

        //Assert
        Assert.Equal("bar", actualModel.Value);
    }

    interface IContract
    {
        [Post("void")]
        public Task PerformVoidAsync([JsonContent]int parameter);

        [Post("add")]
        public Task<int> PerformAddAsync([JsonContent] int parameter);

        [Get("newtonjson")]
        public Task<NewtonJsonModel> GetNewtonJsonModel();

        [Get("microsoft")]
        public Task<MicrosoftModel> GetMicrosoftModel();
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