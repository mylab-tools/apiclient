using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

namespace MyLab.ApiClient.ResponseProcessing;

/// <summary>
/// Contains detailed service call information 
/// </summary>
public class CallDetails
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public required HttpStatusCode StatusCode { get; init; }
    /// <summary>
    /// Indicates whether the service call was successful based on the HTTP status code.
    /// </summary>
    public required bool IsOK { get; init; }
    /// <summary>
    /// Text request dump
    /// </summary>
    public required string RequestDump { get; init; }
    /// <summary>
    /// Text response dump
    /// </summary>
    public required string ResponseDump { get; init; }
    /// <summary>
    /// Response object
    /// </summary>
    public required HttpResponseMessage ResponseMessage { get; init; }
    /// <summary>
    /// Request object
    /// </summary>
    public required HttpRequestMessage RequestMessage { get; init; }

    /// <summary>
    /// Throws <see cref="ResponseCodeException"/> if <see cref="IsOK"/> is false
    /// </summary>
    public async Task ThrowIfNotOKAsync()
    {
        if (IsOK)
            return;

        throw await ResponseCodeException.FromResponseMessage(ResponseMessage);
    }

    /// <summary>
    /// Reads and deserializes the content of the HTTP response message into an object of the specified type.
    /// </summary>
    public async Task<T?> ReadContentAsync<T>()
    {
        var deserializer = SupportedContentDeserializers.Instance.GetRequiredDeserializer(typeof(T));
        var target = await deserializer.DeserializeAsync(ResponseMessage.Content, typeof(T));

        return target == null ? default : (T)target;
    }
}