using System;
using System.Dynamic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Options;

namespace MyLab.ApiClient;

/// <summary>
/// Provides extension methods for configuring API client options in an <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>
/// This class contains methods to simplify the configuration of API client options, 
/// including binding configuration sections or using custom configuration actions.
/// </remarks>
public static class ConfigServiceCollectionExtensions
{
    /// <param name="services">The <see cref="IServiceCollection"/> to which the configuration will be added.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Configures the API client options for the application using the specified configuration section.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> instance containing the configuration data.</param>
        /// <param name="sectionName">
        /// The name of the configuration section to bind to the <see cref="ApiClientOptions"/>. 
        /// Defaults to <see cref="ApiClientOptions.DefaultSectionName"/> if not specified.
        /// </param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="config"/> or <paramref name="sectionName"/> is <c>null</c>.
        /// </exception>
        public IServiceCollection ConfigureApiClient(IConfiguration config, string sectionName = ApiClientOptions.DefaultSectionName)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (sectionName == null)
                throw new ArgumentNullException(nameof(sectionName));

           
            var optionsSection = config.GetSection(sectionName);

            services.Configure<ApiClientOptions>(opt => ApiClientOptions.FillFromSection(opt, optionsSection));

            return services;
        }

        /// <summary>
        /// Configures an API client endpoint with the specified binding name and configuration.
        /// </summary>
        /// <param name="bindingKey">
        /// The name of the binding to associate with the endpoint configuration.
        /// </param>
        /// <param name="config">
        /// The <see cref="IConfiguration"/> instance containing the endpoint configuration.
        /// </param>
        /// <param name="endpointSectionName">
        /// The name of the configuration section that contains the endpoint settings.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance to allow for method chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="config"/> or <paramref name="endpointSectionName"/> is <c>null</c>.
        /// </exception>
        public IServiceCollection ConfigureApiClientEndpoint(string bindingKey, IConfiguration config, string endpointSectionName)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (endpointSectionName == null)
                throw new ArgumentNullException(nameof(endpointSectionName));

            var endpointSection = config.GetSection(endpointSectionName);

            services.Configure<ApiClientOptions>(opt =>
            {
                var epOptions = ApiClientOptions.GetEndpointOptions(endpointSection);
                opt.Endpoints[bindingKey] = epOptions;
            });

            return services;
        }

        /// <summary>
        /// Configures an API client endpoint for the specified contract type using the provided configuration.
        /// </summary>
        /// <typeparam name="TContract">
        /// The type of the contract associated with the API client endpoint.
        /// </typeparam>
        /// <param name="config">
        /// The <see cref="IConfiguration"/> instance containing the endpoint configuration.
        /// </param>
        /// <param name="endpointSectionName">
        /// The name of the configuration section that contains the endpoint settings.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance to allow for method chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="config"/> or <paramref name="endpointSectionName"/> is <c>null</c>.
        /// </exception>
        public IServiceCollection ConfigureApiClientEndpoint<TContract>(IConfiguration config,
            string endpointSectionName)
        {
            return services.ConfigureApiClientEndpoint(typeof(TContract).FullName!, config, endpointSectionName);
        }

        /// <summary>
        /// Configures API client options for the application.
        /// </summary>
        /// <param name="configureOptions">
        /// An <see cref="Action{T}"/> to configure the <see cref="ApiClientOptions"/>.
        /// </param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="configureOptions"/> is <c>null</c>.
        /// </exception>
        public IServiceCollection ConfigureApiClient(Action<ApiClientOptions> configureOptions)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);

            return services;
        }

        #region Obsolete

        /// <summary>
        /// Configures ApiClient factoring
        /// </summary>
        [Obsolete($"Use {nameof(ConfigureApiClient)} instead", true)]
        public IServiceCollection ConfigureApiClients(
            IConfiguration config,
            string sectionName = "")
        {
            throw new NotImplementedException("Obsolete");
        }

        /// <summary>
        /// Configures ApiClient factoring
        /// </summary>
        [Obsolete($"Use {nameof(ConfigureApiClient)} instead", true)]
        public IServiceCollection ConfigureApiClients(
            Action<ApiClientOptions> configureOptions)
        {
            throw new NotImplementedException("Obsolete");
        }

        #endregion
    }
}