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

    class DefaultApiContractRegistrar : IApiContractRegistrar
    {
        private readonly IServiceCollection _services;

        readonly List<string> _registeredApiKeys = new List<string>();

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultApiContractRegistrar"/>
        /// </summary>
        public DefaultApiContractRegistrar(IServiceCollection services)
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
}
