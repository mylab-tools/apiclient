using System;
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
    public void ShouldThrowWhenInvokerEndpointIsNotConfigured()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        //Assert
        Assert.Throws<InvalidOperationException>(() => sp.GetRequiredService<IApiClientInvoker<IContract>>());
    }

    [Fact]
    public void ShouldThrowWhenInvokerEndpointIsNotConfiguredScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>()
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        using var scope = sp.CreateScope();

        //Assert
        Assert.Throws<InvalidOperationException>(() =>
            scope.ServiceProvider.GetRequiredService<IApiClientInvoker<IContract>>());
    }
}