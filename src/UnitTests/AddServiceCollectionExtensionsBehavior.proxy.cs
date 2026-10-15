using System;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using MyLab.ApiClient.Usage;
using Xunit;

namespace UnitTests;

public partial class AddServiceCollectionExtensionsBehavior
{
    [Fact]
    public void ShouldInjectProxyAsContractImplementation()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, "foo"))
            .BuildServiceProvider();

        var client = sp.GetRequiredService<IContract>();
        var proxy = client as ApiClientProxy;
        var requestProcessor = proxy?.RequestProcLogic?.RequestProcessor as HttpClientRequestProcessor;
        var httpClientProvider = requestProcessor?.HttpClientProvider;
        var httpClient = httpClientProvider?.Provide();

        //Assert
        Assert.NotNull(proxy);
        Assert.NotNull(requestProcessor);
        Assert.NotNull(httpClientProvider);
        CheckHttpClientForBaseAddress(httpClient);
    }

    [Fact]
    public void ShouldInjectProxyAsContractImplementationScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, "foo"))
            .BuildServiceProvider();

        IContract scopedContract;

        using (var scope = sp.CreateScope())
            scopedContract = scope.ServiceProvider.GetRequiredService<IContract>();

        var proxy = scopedContract as ApiClientProxy;
        var requestProcessor = proxy?.RequestProcLogic?.RequestProcessor as HttpClientRequestProcessor;
        var httpClientProvider = requestProcessor?.HttpClientProvider;
        var httpClient = httpClientProvider?.Provide();

        //Assert
        Assert.NotNull(proxy);
        Assert.NotNull(requestProcessor);
        Assert.NotNull(httpClientProvider);
        CheckHttpClientForBaseAddress(httpClient);
    }

    [Fact]
    public void ShouldThrowWhenProxyEndpointIsNotConfigured()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        //Assert
        Assert.Throws<InvalidOperationException>(() => sp.GetRequiredService<IContract>());
    }

    [Fact]
    public void ShouldThrowWhenProxyEndpointIsNotConfiguredScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>()
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        using var scope = sp.CreateScope();

        //Assert
        Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<IContract>());
    }
}