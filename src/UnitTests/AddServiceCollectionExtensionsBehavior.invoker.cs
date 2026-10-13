using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using MyLab.ApiClient.Usage;
using MyLab.ApiClient.Usage.Invocation;
using Xunit;

namespace UnitTests;

public partial class AddServiceCollectionExtensionsBehavior
{
    [Fact]
    public void ShouldInjectInvokerAsContractImplementation()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, "foo"))
            .BuildServiceProvider();

        var invoker = sp.GetRequiredService<IApiClientInvoker<IContract>>();
        var invokerImpl = invoker as ApiClientInvoker<IContract>;
        var requestProcessor = invokerImpl?.RequestProcLogic.RequestProcessor as HttpClientRequestProcessor;
        var httpClientProvider = requestProcessor?.HttpClientProvider;
        var httpClient = httpClientProvider?.Provide();

        //Assert
        Assert.NotNull(invokerImpl);
        Assert.NotNull(requestProcessor);
        Assert.NotNull(httpClientProvider);
        CheckHttpClientForBaseAddress(httpClient);
    }

    [Fact]
    public void ShouldInjectInvokerAsContractImplementationScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, "foo"))
            .BuildServiceProvider();

        IApiClientInvoker<IContract> scopedInvoker;

        using (var scope = sp.CreateScope())
            scopedInvoker = scope.ServiceProvider.GetRequiredService<IApiClientInvoker<IContract>>();

        var invokerImpl = scopedInvoker as ApiClientInvoker<IContract>;
        var requestProcessor = invokerImpl?.RequestProcLogic.RequestProcessor as HttpClientRequestProcessor;
        var httpClientProvider = requestProcessor?.HttpClientProvider;
        var httpClient = httpClientProvider?.Provide();

        //Assert
        Assert.NotNull(invokerImpl);
        Assert.NotNull(requestProcessor);
        Assert.NotNull(httpClientProvider);
        CheckHttpClientForBaseAddress(httpClient);
    }

    [Fact]
    public void ShouldInjectOptionalInvokerAsContractImplementation()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>(optional: true)
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        var invoker = sp.GetService<IApiClientInvoker<IContract>>();

        //Assert
        Assert.Null(invoker);
    }

    [Fact]
    public void ShouldInjectOptionalInvokerAsContractImplementationScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>(optional: true)
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        IApiClientInvoker<IContract> scopedInvoker;

        using (var scope = sp.CreateScope())
            scopedInvoker = scope.ServiceProvider.GetService<IApiClientInvoker<IContract>>();

        //Assert
        Assert.Null(scopedInvoker);
    }
}