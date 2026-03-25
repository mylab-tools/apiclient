using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Options;
using System;
using System.Net.Http;
using MyLab.ApiClient.Usage;

namespace MyLab.ApiClient.DI;

class ApiContractRegistrar<TContract> where TContract : class
{
    readonly ApiContractBinding _binding = new(typeof(TContract));
        
    public bool Optional { get; set; } = false;

    public void Register(IServiceCollection services)
    {
        ContractValidator.Validate(typeof(TContract));

        services.AddSingleton(serviceProvider =>
        {
            var opts = serviceProvider.GetService<IOptions<ApiClientOptions>>();

            if (opts?.Value == null)
                throw new InvalidOperationException("ApiClient options not found");

            if (!_binding.TryGetOptions(opts.Value, out _, out var bindingKey))
            {
                if (Optional)
                {
                    return null;
                }

                throw new InvalidOperationException($"Api '{typeof(TContract).FullName}' options not found");
            }

            var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClientProvider = new FactoryHttpClientProvider(httpFactory, bindingKey!);
            IRequestProcessor reqProc = new HttpClientRequestProcessor(httpClientProvider);

            return ApiClientProxy.CreateForContract<TContract>(reqProc, opts.Value);
        });
    }

    public void RegisterScoped(IServiceCollection services)
    {
        ContractValidator.Validate(typeof(TContract));

        services.AddScoped(serviceProvider =>
        {
            var opts = serviceProvider.GetService<IOptions<ApiClientOptions>>();

            if (opts?.Value == null)
                throw new InvalidOperationException("ApiClient options not found");

            if (!_binding.TryGetOptions(opts.Value, out _, out var bindingKey))
            {
                if (Optional)
                {
                    return null;
                }

                throw new InvalidOperationException($"Api '{typeof(TContract).FullName}' options not found");
            }

            var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClientProvider = SingleHttpClientProvider.CrateFromHttpClientFactory(httpFactory, bindingKey!);
            IRequestProcessor reqProc = new HttpClientRequestProcessor(httpClientProvider);

            return ApiClientProxy.CreateForContract<TContract>(reqProc, opts.Value);
        });
    }
}