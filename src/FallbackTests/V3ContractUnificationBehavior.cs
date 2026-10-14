using System.Linq;
using System.Net.Http;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.DI;
using MyLab.ApiClient.Options;
using V3Contracts;
using Xunit;

namespace FallbackTests;

public class V3ContractUnificationBehavior
{
    [Fact]
    public void ServiceModel_ShouldReadPrebuiltV3Contract()
    {
        var model = ServiceModel.FromContract(typeof(ILegacyOrdersApi));

        Assert.Contains("legacy-orders", model.Bindings!);
        Assert.Equal(2, model.Endpoints.Count);

        var get = model.Endpoints.Values.Single(e => e.HttpMethod == HttpMethod.Get);
        Assert.Equal("items/{id}", get.Url);
        Assert.Equal(2, get.Parameters.Count);

        var post = model.Endpoints.Values.Single(e => e.HttpMethod == HttpMethod.Post);
        Assert.Equal("items", post.Url);
        Assert.Equal(2, post.Parameters.Count);
    }

    [Fact]
    public void ApiContractBinding_ShouldResolvePrebuiltV3Key()
    {
        var binding = new ApiContractBinding(typeof(ILegacyOrdersApi));
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
}
