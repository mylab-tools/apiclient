using AutoFixture.Xunit3;
using MyLab.ApiClient.Test;
using MyLab.ApiClient.Usage.Invocation;
using TestServer;

namespace E2eTests;

public class ApiClientInvokerBehavior : IClassFixture<TestApiFixture<Program, ITestServerContract>>
{
    readonly ApiClientInvoker<ITestServerContract> _invoker;

    public ApiClientInvokerBehavior(TestApiFixture<Program, ITestServerContract> apiFxt, ITestOutputHelper output)
    {
        apiFxt.Output = output;
        _invoker = apiFxt.StartAppWithInvoker().Invoker;
    }

    [Theory, AutoData]
    public async Task ShouldTransferJsonPayload(TestDto initial)
    {
        //Arrange

        //Act
        var callDetails = await _invoker
            .ForExpression(c => c.GetJsonDto(initial))
            .InvokeAsync(TestContext.Current.CancellationToken);

        var actual = await callDetails.ReadContentAsync<TestDto>();

        //Assert
        Assert.Equal(initial, actual);
    }

    [Theory, AutoData]
    public async Task ShouldTransferXmlPayload(TestDto initial)
    {
        //Arrange

        //Act
        var callDetails = await _invoker
            .ForExpression(c => c.GetXmlDto(initial))
            .InvokeAsync(TestContext.Current.CancellationToken);

        var actual = await callDetails.ReadContentAsync<TestDto>();

        //Assert
        Assert.Equal(initial, actual);
    }
}
