using MyLab.ApiClient.Usage.Invocation;

namespace MyLab.ApiClient.Test;

/// <summary>
/// Contains tst API tools and <see cref="ApiClientInvoker{TContact}"/> intended for interaction with API
/// </summary>
/// <typeparam name="TContract">API contract</typeparam>
public class InvokerTestApiAsset<TContract> : TestApiAsset
    where TContract : class
{
    /// <summary>
    /// A client intended for interaction with API
    /// </summary>
    public required ApiClientInvoker<TContract> Invoker { get; init; }
}