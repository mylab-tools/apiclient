using System;
using System.Net.Http;
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
            var validationIssues = MarkupValidator.Validate(typeof(TContract));
            if (validationIssues != null)
                throw new ApiContractValidationException(validationIssues);

            _services.AddScoped(serviceProvider =>
            {
                var httpFactory = (IHttpClientFactory) serviceProvider.GetService(typeof(IHttpClientFactory));
                return ApiProxy<TContract>.Create(new FactoryHttpClientProvider(httpFactory));
            });
        }
    }
}
