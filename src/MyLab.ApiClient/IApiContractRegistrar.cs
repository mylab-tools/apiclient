using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultApiContractRegistrar"/>
        /// </summary>
        public DefaultApiContractRegistrar(IServiceCollection services)
        {
            _services = services;
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
                var httpFactory = (IHttpClientFactory) serviceProvider.GetService(typeof(IHttpClientFactory));
                return ApiProxy<TContract>.Create(new FactoryHttpClientProvider(httpFactory, serviceKey));
            });
        }
    }
}
