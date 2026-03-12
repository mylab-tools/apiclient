using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Usage;
using System.Net.Http;
using Xunit;

namespace MyLab.ApiClient.Tests;

[TestSubject(typeof(AddServiceCollectionExtensions))]
public partial class AddServiceCollectionExtensionsBehavior
{
    [Theory]
    [MemberData(nameof(GetBindingKeysCases))]
    public void ShouldConfigureApiClientWithBindingKeys(string bindingKey)
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, bindingKey))
            .BuildServiceProvider();

        var httpClient = sp
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient(bindingKey);

        //Assert
        CheckHttpClientForBaseAddress(httpClient);
    }
    
    [Fact]
    public void ShouldFailWithWrongBindingKey()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>()
            .ConfigureApiClient(opt => AddEndpoint(opt, "foo"))
            .BuildServiceProvider();

        var httpClient = sp
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient("bar");

        //Assert
        Assert.NotNull(httpClient);
        Assert.Null(httpClient.BaseAddress);
    }

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
        var requestProcessor = proxy?.RequestProcessor as HttpClientRequestProcessor;
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
        var requestProcessor = proxy?.RequestProcessor as HttpClientRequestProcessor;
        var httpClientProvider = requestProcessor?.HttpClientProvider;
        var httpClient = httpClientProvider?.Provide();

        //Assert
        Assert.NotNull(proxy);
        Assert.NotNull(requestProcessor);
        Assert.NotNull(httpClientProvider);
        CheckHttpClientForBaseAddress(httpClient);
    }

    [Fact] public void ShouldInjectOptionalProxyAsContractImplementation()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.AddApiClient<IContract>(optional: true)
            .ConfigureApiClient(_ => {})
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
            .ConfigureApiClient(_ => {})
            .BuildServiceProvider();

        IContract scopedContract;

        using (var scope = sp.CreateScope())
            scopedContract = scope.ServiceProvider.GetService<IContract>();

        //Assert
        Assert.Null(scopedContract);
    }
}