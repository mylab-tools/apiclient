using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using System;
using System.Net.Http;
using MyLab.ApiClient.Usage;

namespace MyLab.ApiClient.DI
{
    class ApiContractRegistrar<TContract>
    {
        readonly ApiContractBinding _binding = new(typeof(TContract));
        
        public bool Optional { get; set; } = false;

        public void Regster(IServiceCollection services)
        {
            ContractValidator.Validate(typeof(TContract));

            services.AddSingleton(serviceProvider =>
            {
                var opts = serviceProvider.GetService<IOptions<ApiClientOptions>>();

                if (opts?.Value == null)
                    throw new InvalidOperationException("ApiClient options not found");

                if (!_binding.TryGetOptions(opts.Value, out var epOpt))
                {
                    return Optional
                        ? null
                        : throw new InvalidOperationException($"Api '{typeof(TContract).FullName}' options not found");
                }

                var reqFactoringSettings = RequestFactoringSettings.CreateFromOptions(opts.Value);
                var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

                IRequestProcessor reqProc =
                
                return ApiClientProxy.CreateFroContract<TContract>()
            });
        }
    }
}
