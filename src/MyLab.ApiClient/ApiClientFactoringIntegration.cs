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

            var optionsSection = configuration.GetSection(sectionName);

            if(!optionsSection.Exists())
                throw new InvalidOperationException($"Section '{sectionName}' does not exists");

            var opts = optionsSection.Get<ApiClientsOptions>();

            services
                .AddSingleton<IApiClientFactory, ApiClientFactory>()
                .Configure<ApiClientsOptions>(optionsSection);

            HttpClientRegistrar.Register(services, opts);

            if (contractRegistration != null)
            {
                var contractRegistrar = new DefaultApiContractRegistrar(services, opts);
                contractRegistration(contractRegistrar);
            }

            return services;
        }

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar>? contractRegistration,
            Action<ApiClientsOptions>? configureOptions,
            IHttpClientFactory clientFactory = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton<IApiClientFactory, ApiClientFactory>();

            if (clientFactory != null)
            {
                services.AddSingleton(clientFactory);
            }

            var opts = new ApiClientsOptions();

            if (configureOptions != null)
            {
                services.Configure(configureOptions);
                configureOptions(opts);
            }

            HttpClientRegistrar.Register(services, opts);

            if (contractRegistration != null)
            {
                var contractRegistrar = new DefaultApiContractRegistrar(services, opts);
                contractRegistration(contractRegistrar);
            }

            return services;
        }
    }
}
