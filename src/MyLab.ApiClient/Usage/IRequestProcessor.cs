using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage;

/// <summary>
/// Defines a mechanism for processing HTTP requests in an API client.
/// </summary>
public interface IRequestProcessor
{
    /// <summary>
    /// Processes an HTTP request asynchronously and returns the corresponding HTTP response.
    /// </summary>
    Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestMessage request);
}