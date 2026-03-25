namespace MyLab.ApiClient.Test;

/// <summary>
/// Contains tools to operate with test API
/// </summary>
public class TestApiAsset
{
    /// <summary>
    /// Application service provider
    /// </summary>
    public required IServiceProvider ServiceProvider { get; init; }
    /// <summary>
    /// An <see cref="HttpClient"/> which will be used for interaction with API
    /// </summary>
    public required HttpClient HttpClient { get; init; }
}