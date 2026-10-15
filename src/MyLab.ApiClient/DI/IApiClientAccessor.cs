using MyLab.ApiClient.Usage.Invocation;

namespace MyLab.ApiClient.DI;

/// <summary>
/// Provides access to the API client and invoker registered for a contract.
/// </summary>
/// <typeparam name="TContract">
/// The API contract interface associated with the client.
/// </typeparam>
/// <remarks>
/// The accessor is registered together with the contract and is never <c>null</c>.
/// Unlike direct injection of <typeparamref name="TContract"/> or
/// <see cref="IApiClientInvoker{TContract}"/>, it exposes both the proxy and the invoker.
/// </remarks>
public interface IApiClientAccessor<TContract>
    where TContract : class
{
    /// <summary>
    /// Gets a value indicating whether the client was created.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Gets the transparent proxy for <typeparamref name="TContract"/>.
    /// </summary>
    TContract? Client { get; }

    /// <summary>
    /// Gets the detailed invoker for <typeparamref name="TContract"/>.
    /// </summary>
    IApiClientInvoker<TContract>? Invoker { get; }
}
