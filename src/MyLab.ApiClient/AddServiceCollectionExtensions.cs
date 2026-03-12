using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.DI;
using MyLab.ApiClient.Options;
using System;
using System.Net.Http;

namespace MyLab.ApiClient;

public static class AddServiceCollectionExtensions
{
    /// <param name="services">The <see cref="IServiceCollection"/> to which the configuration will be added.</param>
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApiClient<TContract>(bool optional = false)
            where TContract : class
        {
            var httpClientRegistrar = new HttpClientRegistrar<TContract>
            {
                Optional = optional
            };
            
            httpClientRegistrar.Register(services);

            return services;
        }

        public IServiceCollection AddScopedApiClient<TContract>(bool optional = false)
            where TContract : class
        {
            var httpClientRegistrar = new HttpClientRegistrar<TContract>
            {
                Optional = optional
            };

            httpClientRegistrar.Register(services);

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
        [Obsolete($"Use {nameof(AddScopedApiClient)} instead", true)]
        public  IServiceCollection AddScopedApiClients(
            Action<IApiContractRegistrar> contractRegistration)
        {
            throw new NotImplementedException("Obsolete");
        }

        #endregion
    }
}