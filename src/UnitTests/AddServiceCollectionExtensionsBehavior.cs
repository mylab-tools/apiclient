using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using MyLab.ApiClient.Usage;
using Xunit;

namespace UnitTests;

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
}