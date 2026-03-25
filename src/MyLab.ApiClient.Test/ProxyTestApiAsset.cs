namespace MyLab.ApiClient.Test;

/// <summary>
/// Contains tst API tools and proxy intended for interaction with API
/// </summary>
/// <typeparam name="TContract">API contract</typeparam>
public class ProxyTestApiAsset<TContract> : TestApiAsset
    where TContract : class
{
    /// <summary>
    /// A client intended for interaction with API
    /// </summary>
    public required TContract Proxy { get; init; }
}