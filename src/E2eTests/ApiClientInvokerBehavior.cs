using AutoFixture.Xunit3;
using MyLab.ApiClient.Test;
using TestServer;

namespace E2eTests;

public class ApiClientInvokerBehavior : IClassFixture<TestApiFixture<Program, ITestServerContract>>
{
    readonly TestApiFixture<Program, ITestServerContract> _fxt;

    public ApiClientInvokerBehavior(TestApiFixture<Program, ITestServerContract> apiFxt, ITestOutputHelper output)
    {
        _fxt = apiFxt;
        _fxt.Output = output;
    }

    [Theory, AutoData]
    public async Task ShouldTransferJsonPayload(TestDto initial)
    {
        //Arrange
        using var asset = _fxt.StartAppWithInvoker();

        //Act
        var callDetails = await asset.Invoker
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
        using var asset = _fxt.StartAppWithInvoker();

        //Act
        var callDetails = await asset.Invoker
            .ForExpression(c => c.GetXmlDto(initial))
            .InvokeAsync(TestContext.Current.CancellationToken);

        var actual = await callDetails.ReadContentAsync<TestDto>();

        //Assert
        Assert.Equal(initial, actual);
    }
}
