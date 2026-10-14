using System.Threading.Tasks;
using MyLab.ApiClient;

namespace V3Contracts;

/// <summary>
/// Contract compiled against MyLab.ApiClient 3.x attribute types.
/// </summary>
[Api(Key = "legacy-orders")]
public interface ILegacyOrdersApi
{
    [Get("items/{id}")]
    Task<string> GetItem([Path] string id, [Query] int? take);

    [Post("items")]
    Task Create([JsonContent] object body, [Header("X-Trace")] string trace);
}
