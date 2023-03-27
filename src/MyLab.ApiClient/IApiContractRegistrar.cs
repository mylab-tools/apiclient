using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Registers api contract 
    /// </summary>
    public interface IApiContractRegistrar
    {
        /// <summary>
        /// Registers api contract 
        /// </summary>
        void RegisterContract<TContract>()
            where TContract : class;
    }

    class KeyBasedApiContractRegistrar : IApiContractRegistrar
    {
        private readonly IServiceCollection _services;

        readonly List<string> _registeredApiKeys = new List<string>();

        /// <summary>
        /// Initializes a new instance of <see cref="KeyBasedApiContractRegistrar"/>
        /// </summary>
        public KeyBasedApiContractRegistrar(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerable<string> GetRegisteredApiKeys()
        {
            return _registeredApiKeys;
        }

        public void RegisterContract<TContract>()
            where TContract : class
        {
            var validationResult = new ApiContractValidator
            {
                ContractKeyMustBeSpecified = true
            }.Validate(typeof(TContract));

            if (!validationResult.Success)
                throw new ApiContractValidationException(validationResult);

            string serviceKey = typeof(TContract).GetCustomAttribute<ApiAttribute>()?.Key;
            
            _services.AddSingleton(serviceProvider =>
            {
                var opts = serviceProvider.GetService<IOptions<ApiClientsOptions>>();
                
                var reqFactoringSettings = opts.Value != null
                    ? RequestFactoringSettings.CreateFromOptions(opts.Value)
                    : null;

                var httpFactory = (IHttpClientFactory) serviceProvider.GetService(typeof(IHttpClientFactory));
                return ApiProxy<TContract>.Create(new FactoryHttpClientProvider(httpFactory, serviceKey), reqFactoringSettings);
            });

            _registeredApiKeys.Add(serviceKey);
        }
    }

    class ScopedApiContractRegistrar : IApiContractRegistrar
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of <see cref="ScopedApiContractRegistrar"/>
        /// </summary>
        public ScopedApiContractRegistrar(IServiceCollection services)
        {
            _services = services;
        }

        public void RegisterContract<TContract>()
            where TContract : class
        {
            var validationResult = new ApiContractValidator
            {
                ContractKeyMustBeSpecified = false
            }.Validate(typeof(TContract));

            if (!validationResult.Success)
                throw new ApiContractValidationException(validationResult);

            _services.AddScoped(serviceProvider =>
            {
                var opts = serviceProvider.GetService<IOptions<ApiClientsOptions>>();

                var reqFactoringSettings = opts?.Value != null
                    ? RequestFactoringSettings.CreateFromOptions(opts.Value)
                    : null;

                var httpClient = serviceProvider.GetService<HttpClient>();
                return ApiProxy<TContract>.Create(new SingleHttpClientProvider(httpClient), reqFactoringSettings);
            });
        }
    }
}
