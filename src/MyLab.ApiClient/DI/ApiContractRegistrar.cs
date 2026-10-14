using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Options;
using System;
using System.Net.Http;
using MyLab.ApiClient.Usage;
using MyLab.ApiClient.Usage.Invocation;

namespace MyLab.ApiClient.DI;

class ApiContractRegistrar<TContract> where TContract : class
{
    readonly ApiContractBinding _binding = new(typeof(TContract));
        
    public bool Optional { get; set; } = false;

    public void Register(IServiceCollection services)
    {
        ContractValidator.Validate(typeof(TContract));

        //Inject proxy
        services.AddSingleton(serviceProvider =>
        {
            if (!TryGetFactoringParams(serviceProvider, out var reqProc, out var opts))
            {
                return null;
            }
            
            return ApiClientProxy.CreateForContract<TContract>(reqProc!, opts!);
        });

        //Inject invoker
        services.AddSingleton(serviceProvider =>
        {
            if (!TryGetFactoringParams(serviceProvider, out var reqProc, out var opts))
            {
                return null;
            }

            return ApiClientInvokerFactory.FromContract<TContract>(reqProc!, opts!);
        });

        //Inject accessor (always non-null; Client/Invoker may be null when optional)
        services.AddSingleton<IApiClientAccessor<TContract>>(CreateAccessor);
    }

    public void RegisterScoped(IServiceCollection services)
    {
        ContractValidator.Validate(typeof(TContract));

        //Inject proxy
        services.AddScoped(serviceProvider =>
        {
            if (!TryGetFactoringParams(serviceProvider, out var reqProc, out var opts))
            {
                return null;
            }

            return ApiClientProxy.CreateForContract<TContract>(reqProc!, opts!);
        });

        //Inject invoker
        services.AddScoped(serviceProvider =>
        {
            if (!TryGetFactoringParams(serviceProvider, out var reqProc, out var opts))
            {
                return null;
            }

            return ApiClientInvokerFactory.FromContract<TContract>(reqProc!, opts!);
        });

        //Inject accessor (always non-null; Client/Invoker may be null when optional)
        services.AddScoped<IApiClientAccessor<TContract>>(CreateAccessor);
    }

    static IApiClientAccessor<TContract> CreateAccessor(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetService<TContract>();
        var invoker = serviceProvider.GetService<IApiClientInvoker<TContract>>();

        return new ApiClientAccessor<TContract>(client, invoker);
    }

    bool TryGetFactoringParams(IServiceProvider serviceProvider, out IRequestProcessor? reqProc, out ApiClientOptions? opts)
    {
        opts = serviceProvider.GetService<IOptions<ApiClientOptions>>()?.Value;

        if (opts == null)
            throw new InvalidOperationException("ApiClient options not found");

        if (!_binding.TryGetOptions(opts, out _, out var bindingKey))
        {
            if (Optional)
            {
                reqProc = null;
                return false;
            }

            throw new InvalidOperationException($"Api '{typeof(TContract).FullName}' options not found");
        }

        var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClientProvider = new FactoryHttpClientProvider(httpFactory, bindingKey!);
        reqProc = new HttpClientRequestProcessor(httpClientProvider);

        return true;
    }
}
