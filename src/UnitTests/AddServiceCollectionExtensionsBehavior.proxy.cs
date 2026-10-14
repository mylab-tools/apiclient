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
    public void ShouldInjectOptionalProxyAsContractImplementation()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>(optional: true)
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        var client = sp.GetService<IContract>();

        //Assert
        Assert.Null(client);
    }

    [Fact]
    public void ShouldInjectOptionalProxyAsContractImplementationScoped()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClientScoped<IContract>(optional: true)
            .ConfigureApiClient(_ => { })
            .BuildServiceProvider();

        IContract scopedContract;

        using (var scope = sp.CreateScope())
            scopedContract = scope.ServiceProvider.GetService<IContract>();

        //Assert
        Assert.Null(scopedContract);
    }
}