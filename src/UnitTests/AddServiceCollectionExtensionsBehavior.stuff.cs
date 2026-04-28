using System.Net.Http;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Options;
using Xunit;

namespace UnitTests;

public partial class AddServiceCollectionExtensionsBehavior
{
    void AddEndpoint(ApiClientOptions opt, string bindingKey)
    {
        opt.Endpoints.Add
        (
            bindingKey,
            new ApiEndpointOptions
            {
                Url = "http://foo.com"
            }
        );
    }

    void CheckHttpClientForBaseAddress(HttpClient? httpClient)
    {
        Assert.NotNull(httpClient);
        Assert.NotNull(httpClient.BaseAddress);
        Assert.Equal("http://foo.com", httpClient.BaseAddress!.ToString().TrimEnd('/'));
    }

    public static object[][] GetBindingKeysCases()
    {
        return new[]
        {
            new[] { "foo" },
            new[] { nameof(IContract) },
            new[] { typeof(IContract).FullName },
        };
    }

    [ApiContract(Binding = "foo")]
    interface IContract
    {
    }
}