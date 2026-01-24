using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Options;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides extension methods for configuring API client options in an <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This class contains methods to simplify the configuration of API client options, 
    /// including binding configuration sections or using custom configuration actions.
    /// </remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the API client options for the application using the specified configuration section.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the configuration will be added.</param>
        /// <param name="config">The <see cref="IConfiguration"/> instance containing the configuration data.</param>
        /// <param name="sectionName">
        /// The name of the configuration section to bind to the <see cref="ApiClientOptions"/>. 
        /// Defaults to <see cref="ApiClientOptions.DefaultSectionName"/> if not specified.
        /// </param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="config"/> or <paramref name="sectionName"/> is <c>null</c>.
        /// </exception>
        public static IServiceCollection ConfigureApiClient
        (
            this IServiceCollection services,
            IConfiguration config,
            string sectionName = ApiClientOptions.DefaultSectionName
        )
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (sectionName == null)
                throw new ArgumentNullException(nameof(sectionName));

            var optionsSection = config.GetSection(sectionName);

            services.Configure<ApiClientOptions>(optionsSection);

            return services;
        }

        /// <summary>
        /// Configures API client options for the application.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the configuration will be added.</param>
        /// <param name="configureOptions">
        /// An <see cref="Action{T}"/> to configure the <see cref="ApiClientOptions"/>.
        /// </param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="configureOptions"/> is <c>null</c>.
        /// </exception>
        public static IServiceCollection ConfigureApiClient
        (
            this IServiceCollection services,
            Action<ApiClientOptions> configureOptions
        )
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);

            return services;
        }
    }
}
