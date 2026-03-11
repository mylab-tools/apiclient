using System;
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
public partial class ApiClientProxyBehavior
{
    [Fact]
    public async Task ShouldPerformVoidMethod()
    {
        //Arrange
        var response = CreateOkResponse();
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, _defaultOptions);

        //Act
        await proxy.PerformVoidAsync(1);

        //Assert
        VerifyResponseUrl(reqProcMock, "void");
    }

    [Fact]
    public async Task ShouldPerformWithReturnParameters()
    {
        //Arrange
        var response = CreateOkResponse("foo");
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, _defaultOptions);

        //Act
        var result = await proxy.GetAsync();

        //Assert
        VerifyResponseUrl(reqProcMock, "get");
        Assert.Equal("foo", result);
    }

    [Fact]
    public async Task ShouldFailIfStatusCodeIsnNotOK()
    {
        //Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, _defaultOptions);

        //Act & Assert
        var e = await Assert.ThrowsAsync<ResponseCodeException>(() => proxy.PerformVoidAsync(1));
        Assert.Equal(HttpStatusCode.NotFound, e.StatusCode);
    }

    [Fact]
    public async Task ShouldSupportNewtonJsonModels()
    {
        //Arrange
        var response = CreateOkResponse($"{{\"{NewtonJsonModel.ValuePropertyName}\":\"foo\"}}");
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, _defaultOptions);

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
        var response = CreateOkResponse($"{{\"{MicrosoftModel.ValuePropertyName}\":\"bar\"}}");
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, opt);

        //Act
        var actualModel = await proxy.GetMicrosoftModel();

        //Assert
        Assert.Equal("bar", actualModel.Value);
    }

    [Fact]
    public async Task ShouldSendInt()
    {
        //Arrange
        var response = CreateOkResponse();
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, _defaultOptions);

        //Act
        await proxy.SendInt(42);

        //Assert
        VerifyResponseUrl(reqProcMock, "send/42");
    }

    [Fact]
    public async Task ShouldSendGuid()
    {
        //Arrange
        var guid = Guid.NewGuid();
        var response = CreateOkResponse(guid.ToString("N"));
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, _defaultOptions);

        //Act
        await proxy.SendGuid(guid);

        //Assert
        VerifyResponseUrl(reqProcMock, $"send/{guid:N}");
    }

    [Fact]
    public async Task ShouldSendObject()
    {
        //Arrange
        var response = CreateOkResponse();
        var reqProcMock = CreateReqProcMock(response);
        var proxy = ApiClientProxy.CreateFroContract<IContract>(reqProcMock.Object, _defaultOptions);

        var obj = new NewtonJsonModel { Value = "foo" };

        //Act
        await proxy.SendObj(obj);

        //Assert
        VerifyResponseContent(reqProcMock, $"{{\"{NewtonJsonModel.ValuePropertyName}\":\"foo\"}}");
    }
}