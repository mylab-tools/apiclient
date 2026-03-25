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
        var actual = await _invoker.InvokeAsync(c => c.GetJsonDto(initial))
            .;

        //Assert
        Assert.Equal(initial, actual);
    }

    [Theory, AutoData]
    public async Task ShouldTransferXmlPayload(TestDto initial)
    {
        //Arrange

        //Act
        var actual = await _invoker.GetXmlDto(initial);

        //Assert
        Assert.Equal(initial, actual);
    }
}
