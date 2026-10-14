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
    public void ShouldInjectUnconfiguredOptionalAccessor()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>(optional: true)
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        var accessor = sp.GetRequiredService<IApiClientAccessor<IContract>>();

        //Assert
        Assert.False(accessor.IsConfigured);
        Assert.Null(accessor.Client);
        Assert.Null(accessor.Invoker);
        Assert.Null(sp.GetService<IContract>());
        Assert.Null(sp.GetService<IApiClientInvoker<IContract>>());
    }

    [Fact]
    public void ShouldInjectUnconfiguredOptionalAccessorScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>(optional: true)
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        using var scope = sp.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IApiClientAccessor<IContract>>();

        //Assert
        Assert.False(accessor.IsConfigured);
        Assert.Null(accessor.Client);
        Assert.Null(accessor.Invoker);
    }
}
