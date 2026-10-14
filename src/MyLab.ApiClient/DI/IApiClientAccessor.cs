using MyLab.ApiClient.Usage.Invocation;

namespace MyLab.ApiClient.DI;

/// <summary>
/// Provides safe access to an optionally configured API client and invoker.
/// </summary>
/// <typeparam name="TContract">
/// The API contract interface associated with the client.
/// </typeparam>
/// <remarks>
/// Use this abstraction when the API endpoint may be absent from configuration
/// (<c>AddApiClient&lt;TContract&gt;(optional: true)</c>). Unlike direct injection of
/// <typeparamref name="TContract"/> or <see cref="IApiClientInvoker{TContract}"/>,
/// the accessor itself is always registered and never <c>null</c>.
/// </remarks>
public interface IApiClientAccessor<TContract>
    where TContract : class
{
    /// <summary>
    /// Gets a value indicating whether the API endpoint is present in configuration
    /// and the client was created.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Gets the transparent proxy for <typeparamref name="TContract"/>,
    /// or <c>null</c> when the endpoint is not configured.
    /// </summary>
    TContract? Client { get; }

    /// <summary>
    /// Gets the detailed invoker for <typeparamref name="TContract"/>,
    /// or <c>null</c> when the endpoint is not configured.
    /// </summary>
    IApiClientInvoker<TContract>? Invoker { get; }
}
