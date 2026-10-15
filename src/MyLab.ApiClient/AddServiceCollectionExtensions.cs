using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.DI;
using System;
using System.Net.Http;

namespace MyLab.ApiClient;

/// <summary>
/// Provides extension methods for adding API client services to an <see cref="IServiceCollection"/>.
/// </summary>
public static class AddServiceCollectionExtensions
{
    /// <param name="services">The <see cref="IServiceCollection"/> to which the configuration will be added.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds an API client for the specified contract type to the <see cref="IServiceCollection"/>.
        /// </summary>
        public IServiceCollection AddApiClient<TContract>()
            where TContract : class
        {
            new HttpClientRegistrar<TContract>().Register(services);
            new ApiContractRegistrar<TContract>().Register(services);

            return services;
        }

        /// <summary>
        /// Adds an API client for the specified contract type to the <see cref="IServiceCollection"/> with a scoped lifetime.
        /// </summary>
        public IServiceCollection AddApiClientScoped<TContract>()
            where TContract : class
        {
            new HttpClientRegistrar<TContract>().Register(services);
            new ApiContractRegistrar<TContract>().RegisterScoped(services);

            return services;
        }

        #region Obsolete

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        [Obsolete($"Use {nameof(AddApiClient)} instead", true)]
        public IServiceCollection AddApiClients(
            Action<IApiContractRegistrar> contractRegistration)
        {
            throw new NotImplementedException("Obsolete");
        }

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        [Obsolete($"Use {nameof(AddApiClient)} instead", true)]
        public  IServiceCollection AddOptionalApiClients(
            Action<IApiContractRegistrar> contractRegistration)
        {
            throw new NotImplementedException("Obsolete");
        }

        /// <summary>
        /// Integrates ApiClient factoring
        /// </summary>
        [Obsolete($"Use {nameof(AddApiClient)} instead", true)]
        public IServiceCollection AddApiClients(
            Action<IApiContractRegistrar> contractRegistration,
            IHttpClientFactory clientFactory)
        {
            throw new NotImplementedException("Obsolete");
        }

        /// <summary>
        /// Integrates ApiClient for default HttpClient
        /// </summary>
        [Obsolete($"Use {nameof(AddApiClientScoped)} instead", true)]
        public  IServiceCollection AddScopedApiClients(
            Action<IApiContractRegistrar> contractRegistration)
        {
            throw new NotImplementedException("Obsolete");
        }

        #endregion
    }
}