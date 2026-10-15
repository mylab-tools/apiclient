using System;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using MyLab.ApiClient.DI;
using MyLab.ApiClient.Usage.Invocation;
using Xunit;

namespace UnitTests;

public partial class AddServiceCollectionExtensionsBehavior
{
    [Fact]
    public void ShouldInjectConfiguredAccessor()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, "foo"))
            .BuildServiceProvider();

        var accessor = sp.GetRequiredService<IApiClientAccessor<IContract>>();

        //Assert
        Assert.True(accessor.IsConfigured);
        Assert.NotNull(accessor.Client);
        Assert.NotNull(accessor.Invoker);
        Assert.Same(sp.GetRequiredService<IContract>(), accessor.Client);
        Assert.Same(sp.GetRequiredService<IApiClientInvoker<IContract>>(), accessor.Invoker);
    }

    [Fact]
    public void ShouldInjectConfiguredAccessorScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, "foo"))
            .BuildServiceProvider();

        using var scope = sp.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IApiClientAccessor<IContract>>();

        //Assert
        Assert.True(accessor.IsConfigured);
        Assert.NotNull(accessor.Client);
        Assert.NotNull(accessor.Invoker);
    }

    [Fact]
    public void ShouldThrowWhenAccessorEndpointIsNotConfigured()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        //Assert
        Assert.Throws<InvalidOperationException>(() => sp.GetRequiredService<IApiClientAccessor<IContract>>());
    }

    [Fact]
    public void ShouldThrowWhenAccessorEndpointIsNotConfiguredScoped()
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
            scope.ServiceProvider.GetRequiredService<IApiClientAccessor<IContract>>());
    }
}
