using System;
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
            IConfiguration configuration,
            string sectionName = DefaultConfigSectionName)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (sectionName == null) throw new ArgumentNullException(nameof(sectionName));

            var options = configuration
                .GetSection(sectionName)
                .Get<ApiClientsOptions>();
            
            return AddApiClients(services, options);
        }

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            ApiClientsOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) throw new ArgumentNullException(nameof(options));

            HttpClientRegistrar.Register(services, options);

            return services;
        }
    }
}
