using MyLab.ApiClient.Usage.Invocation;

namespace MyLab.ApiClient.DI;

class ApiClientAccessor<TContract>(
    TContract? client,
    IApiClientInvoker<TContract>? invoker
) : IApiClientAccessor<TContract>
    where TContract : class
{
    public bool IsConfigured => Client != null;

    public TContract? Client { get; } = client;

    public IApiClientInvoker<TContract>? Invoker { get; } = invoker;
}
