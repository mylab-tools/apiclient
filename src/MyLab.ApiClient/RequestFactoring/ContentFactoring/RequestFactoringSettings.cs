using MyLab.ApiClient.JsonSerialization;
using MyLab.ApiClient.Options;
using Newtonsoft.Json;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

/// <summary>
/// Contains request factoring settings
/// </summary>
public class RequestFactoringSettings
{
    /// <summary>
    /// Defines JSON serializer
    /// </summary>
    public IJsonSerializer JsonSerializer { get; init; } = NewtonJsonSerializer.Default;

    /// <summary>
    /// Defines url-encoded-form serialization settings
    /// </summary>
    public ApiUrlFormSettings? UrlFormSettings { get; init; }


    /// <summary>
    /// Creates req factoring settings from app options
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static RequestFactoringSettings CreateFromOptions(ApiClientOptions? options)
    {
        return new RequestFactoringSettings
        {
            JsonSerializer = options?.JsonSerializer ?? NewtonJsonSerializer.Default,
            UrlFormSettings = options?.UrlFormSettings ?? new ApiUrlFormSettings()
        };
    }
}