using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains method to integrate ApiClient factoring
    /// </summary>
    public static class ApiClientFactoringIntegration
    {
        public const string DefaultConfigSectionName = "Api";

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration,
            IConfiguration configuration,
            string sectionName = DefaultConfigSectionName)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (sectionName == null) throw new ArgumentNullException(nameof(sectionName));

            var options = configuration
                .GetSection(sectionName)
                .Get<ApiClientsOptions>();
            
            return AddApiClients(services, contractRegistration, options);
        }

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration,
            ApiClientsOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) throw new ArgumentNullException(nameof(options));

            HttpClientRegistrar.Register(services, options);

            if (contractRegistration != null)
            {
                var contractRegistrar = new DefaultApiContractRegistrar(services);
                contractRegistration(contractRegistrar);
            }

            return services;
        }

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration,
            IHttpClientFactory clientFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));

            services.AddSingleton(clientFactory);

            if (contractRegistration != null)
            {
                var contractRegistrar = new DefaultApiContractRegistrar(services);
                contractRegistration(contractRegistrar);
            }

            return services;
        }
    }
}
