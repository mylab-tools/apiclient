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

    public void Register(IServiceCollection services)
    {
        ContractValidator.Validate(typeof(TContract));

        //Inject proxy
        services.AddSingleton(serviceProvider =>
        {
            GetFactoringParams(serviceProvider, out var reqProc, out var opts);
            return ApiClientProxy.CreateForContract<TContract>(reqProc, opts);
        });

        //Inject invoker
        services.AddSingleton(serviceProvider =>
        {
            GetFactoringParams(serviceProvider, out var reqProc, out var opts);
            return ApiClientInvokerFactory.FromContract<TContract>(reqProc, opts);
        });

        //Inject accessor
        services.AddSingleton(CreateAccessor);
    }

    public void RegisterScoped(IServiceCollection services)
    {
        ContractValidator.Validate(typeof(TContract));

        //Inject proxy
        services.AddScoped(serviceProvider =>
        {
            GetFactoringParams(serviceProvider, out var reqProc, out var opts);
            return ApiClientProxy.CreateForContract<TContract>(reqProc, opts);
        });

        //Inject invoker
        services.AddScoped(serviceProvider =>
        {
            GetFactoringParams(serviceProvider, out var reqProc, out var opts);
            return ApiClientInvokerFactory.FromContract<TContract>(reqProc, opts);
        });

        //Inject accessor
        services.AddScoped(CreateAccessor);
    }

    static IApiClientAccessor<TContract> CreateAccessor(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetService<TContract>();
        var invoker = serviceProvider.GetService<IApiClientInvoker<TContract>>();

        return new ApiClientAccessor<TContract>(client, invoker);
    }

    void GetFactoringParams(IServiceProvider serviceProvider, out IRequestProcessor reqProc, out ApiClientOptions opts)
    {
        var options = serviceProvider.GetService<IOptions<ApiClientOptions>>()?.Value 
                      ?? throw new InvalidOperationException("ApiClient options not found");

        if (!_binding.TryGetOptions(options, out _, out var bindingKey))
            throw new InvalidOperationException($"Api '{typeof(TContract).FullName}' options not found");

        var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClientProvider = new FactoryHttpClientProvider(httpFactory, bindingKey!);
        reqProc = new HttpClientRequestProcessor(httpClientProvider);
        opts = options;
    }
}
