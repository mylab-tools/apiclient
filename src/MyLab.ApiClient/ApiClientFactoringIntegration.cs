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
        /// <summary>
        /// Default configuration section name
        /// </summary>
        public const string DefaultConfigSectionName = "Api";

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration)
        {
            AddServicesCore(services, contractRegistration);
            
            return services;
        }

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        public static IServiceCollection AddOptionalApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (contractRegistration == null) throw new ArgumentNullException(nameof(contractRegistration));

            services.AddSingleton<IApiClientFactory, ApiClientFactory>();

            var contractRegistrar = new OptionalKeyBasedApiContractRegistrar(services);
            contractRegistration(contractRegistrar);

            HttpClientRegistrar.Register(services, contractRegistrar.GetRegisteredApiKeys());

            return services;
        }

        [Obsolete("Use ConfigureApiClient(...) separately")]
        /// <summary>
        /// Integrates ApiClient and configures factoring
        /// </summary>
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration,
            IConfiguration config,
            string sectionName = DefaultConfigSectionName)
        {
            AddServicesCore(services, contractRegistration);

            ConfigureApiClients(services, config, sectionName);

            return services;
        }

        /// <summary>
        /// Integrates ApiClient and configures factoring
        /// </summary>
        [Obsolete("Use ConfigureApiClient(...) separately")]
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration,
            Action<ApiClientsOptions> configureOptions)
        {
            AddServicesCore(services, contractRegistration);

            ConfigureApiClients(services, configureOptions);

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
            AddServicesCore(services, contractRegistration);

            services.AddSingleton(clientFactory);

            return services;
        }

        /// <summary>
        /// Integrates ApiClient for default HttpClient
        /// </summary>
        public static IServiceCollection AddScopedApiClients(
            this IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (contractRegistration == null) throw new ArgumentNullException(nameof(contractRegistration));

            services.AddScoped<IApiClientFactory, ApiClientFactory>();

            var contractRegistrar = new ScopedApiContractRegistrar(services);
            contractRegistration(contractRegistrar);

            HttpClientRegistrar.Register(services, contractRegistrar.GetRegisteredApiKeys());

            return services;
        }

        /// <summary>
        /// Configures ApiClient factoring
        /// </summary>
        public static IServiceCollection ConfigureApiClients(
            this IServiceCollection services,
            IConfiguration config,
            string sectionName = DefaultConfigSectionName)
        {
            if (config == null) 
                throw new ArgumentNullException(nameof(config));
            if (sectionName == null) 
                throw new ArgumentNullException(nameof(sectionName));

            var optionsSection = config.GetSection(sectionName);
            
            services.Configure<ApiClientsOptions>(optionsSection);

            return services;
        }

        /// <summary>
        /// Configures ApiClient factoring
        /// </summary>
        public static IServiceCollection ConfigureApiClients(
            this IServiceCollection services,
            Action<ApiClientsOptions> configureOptions)
        {
            if (configureOptions == null) 
                throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);

            return services;
        }

        static void AddServicesCore(
            IServiceCollection services,
            Action<IApiContractRegistrar> contractRegistration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (contractRegistration == null) throw new ArgumentNullException(nameof(contractRegistration));

            services.AddSingleton<IApiClientFactory, ApiClientFactory>();

            var contractRegistrar = new KeyBasedApiContractRegistrar(services);
            contractRegistration(contractRegistrar);

            HttpClientRegistrar.Register(services, contractRegistrar.GetRegisteredApiKeys());
        }
    }
}
