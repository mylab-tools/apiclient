using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Usage.Invocation;
using Xunit;

namespace MyLab.ApiClient.Tests.Usage.Invocation;

using static TestTools;

[TestSubject(typeof(ApiClientInvoker<>))]
public class ApiClientInvokerBehavior
{
    [Fact]
    public async Task ShouldSendRequest()
    {
        //Arrange
        var successResponse = CreateOkResponse();
        var reqProcMock = CreateReqProcMock(successResponse);
        
        var invoker = ApiClientInvoker<IContract>.FromContract(reqProcMock.Object, DefaultOptions);

        //Act
        var invocationResult = await invoker.InvokeAsync(c => c.Post("foo"));
        var callDetails = invocationResult.CallDetails;
        
        //Assert
        VerifyRequestUrl(reqProcMock, "post");
        VerifyRequestContent(reqProcMock, "foo");
        Assert.True(callDetails.IsOK);
        Assert.Equal(HttpMethod.Post, callDetails.RequestMessage.Method);
        Assert.Equal("post", callDetails.RequestMessage.RequestUri?.ToString());
    }

    [Fact]
    public async Task ShouldReceiveResponse()
    {
        //Arrange
        var successResponse = CreateOkResponse("foo");
        var reqProcMock = CreateReqProcMock(successResponse);

        var invoker = ApiClientInvoker<IContract>.FromContract(reqProcMock.Object, DefaultOptions);

        //Act
        var invocationResult = await invoker.InvokeAsync(c => c.Get());
        var callDetails = invocationResult.CallDetails;

        //Assert
        VerifyRequestUrl(reqProcMock, "get");
        Assert.True(callDetails.IsOK);
        Assert.Equal(HttpMethod.Get, callDetails.RequestMessage.Method);
        Assert.Equal("get", callDetails.RequestMessage.RequestUri?.ToString());
        Assert.Equal("foo", await callDetails.ReadContentAsync<string>());
    }

    interface IContract
    {
        [Post("post")]
        Task Post([StringContent]string content);

        [Get("get")]
        Task<string> Get();
    }
}