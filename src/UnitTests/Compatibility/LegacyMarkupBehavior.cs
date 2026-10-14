#pragma warning disable CS0618 // Legacy MyLab.ApiClient 3.x attributes

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.DI;
using MyLab.ApiClient.Options;
using Xunit;

namespace UnitTests.Compatibility;

public class LegacyMarkupBehavior
{
    [Fact]
    public void ServiceModel_ShouldReadLegacyApiKeyAsBinding()
    {
        var model = ServiceModel.FromContract(typeof(ILegacyContract));

        Assert.Equal("v1", model.Url);
        Assert.Contains("legacy-orders", model.Bindings!);
        Assert.Single(model.Endpoints);
        Assert.Equal(HttpMethod.Get, model.Endpoints.Values.Single().HttpMethod);
        Assert.Equal("items/{id}", model.Endpoints.Values.Single().Url);
    }

    [Fact]
    public void ApiContractBinding_ShouldResolveLegacyKey()
    {
        var binding = new ApiContractBinding(typeof(ILegacyContract));
        var options = new ApiClientOptions
        {
            Endpoints =
            {
                ["legacy-orders"] = new ApiEndpointOptions { Url = "http://legacy.test" }
            }
        };

        var found = binding.TryGetOptions(options, out var endpoint, out var key);

        Assert.True(found);
        Assert.Equal("legacy-orders", key);
        Assert.Equal("http://legacy.test", endpoint!.Url);
    }

    [Fact]
    public void EndpointModel_ShouldAcceptLegacyParameterMarkup()
    {
        var method = typeof(ILegacyContract).GetMethod(nameof(ILegacyContract.GetItem))!;
        var endpoint = EndpointModel.FromMethod(method);

        Assert.Equal(HttpMethod.Get, endpoint.HttpMethod);
        Assert.Equal(2, endpoint.Parameters.Count);
    }

    [Api(Key = "legacy-orders", Url = "v1")]
    interface ILegacyContract
    {
        [Get("items/{id}")]
        Task<string> GetItem([Path] string id, [Query] int? take);
    }
}
