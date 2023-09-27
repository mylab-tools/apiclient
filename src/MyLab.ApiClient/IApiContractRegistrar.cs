using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        void RegisterContract<TContract>(string httpClientName)
            where TContract : class;

        /// <summary>
        /// Registers api contract 
        /// </summary>
        void RegisterContract<TContract>()
            where TContract : class;
    }

    class KeyBasedApiContractRegistrar : IApiContractRegistrar
    {
        private readonly IServiceCollection _services;

        readonly List<string> _registeredApiKeys = new List<string>();

        /// <summary>
        /// Initializes a new instance of <see cref="KeyBasedApiContractRegistrar"/>
        /// </summary>
        public KeyBasedApiContractRegistrar(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerable<string> GetRegisteredApiKeys()
        {
            return _registeredApiKeys;
        }

        public void RegisterContract<TContract>(string httpClientName)
            where TContract : class
        {
            if (httpClientName == null) throw new ArgumentNullException(nameof(httpClientName));
            
            RegisterContractCore<TContract>(httpClientName);
        }

        public void RegisterContract<TContract>() where TContract : class
        {
            RegisterContractCore<TContract>(ApiConfigKeyProvider.Provide(typeof(TContract)));
        }

        void RegisterContractCore<TContract>(string serviceKey) where TContract : class
        {
            var validationResult = new ApiContractValidator().Validate(typeof(TContract));

            if (!validationResult.Success)
                throw new ApiContractValidationException(validationResult);

            _services.AddSingleton(serviceProvider =>
            {
                var opts = serviceProvider.GetService<IOptions<ApiClientsOptions>>();

                var reqFactoringSettings = opts.Value != null
                    ? RequestFactoringSettings.CreateFromOptions(opts.Value)
                    : null;

                var httpFactory = (IHttpClientFactory)serviceProvider.GetService(typeof(IHttpClientFactory));
                return ApiProxy<TContract>.Create(new FactoryHttpClientProvider(httpFactory, serviceKey), reqFactoringSettings);
            });

            _registeredApiKeys.Add(serviceKey);
        }
    }

    class OptionalKeyBasedApiContractRegistrar : IApiContractRegistrar
    {
        private readonly IServiceCollection _services;

        readonly List<string> _registeredApiKeys = new List<string>();

        /// <summary>
        /// Initializes a new instance of <see cref="OptionalKeyBasedApiContractRegistrar"/>
        /// </summary>
        public OptionalKeyBasedApiContractRegistrar(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerable<string> GetRegisteredApiKeys()
        {
            return _registeredApiKeys;
        }

        public void RegisterContract<TContract>(string httpClientName)
            where TContract : class
        {
            if (httpClientName == null) throw new ArgumentNullException(nameof(httpClientName));

            RegisterContractCore<TContract>(httpClientName);
        }

        public void RegisterContract<TContract>() where TContract : class
        {
            RegisterContractCore<TContract>(ApiConfigKeyProvider.Provide(typeof(TContract)));
        }

        void RegisterContractCore<TContract>(string serviceKey) where TContract : class
        {
            var validationResult = new ApiContractValidator().Validate(typeof(TContract));

            if (!validationResult.Success)
                throw new ApiContractValidationException(validationResult);
            
            _services.AddSingleton(serviceProvider =>
            {
                var opts = serviceProvider.GetService<IOptions<ApiClientsOptions>>();

                if (opts?.Value == null || !opts.Value.List.ContainsKey(serviceKey))
                    return null;

                var reqFactoringSettings = RequestFactoringSettings.CreateFromOptions(opts.Value);
                var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

                return ApiProxy<TContract>.Create(new FactoryHttpClientProvider(httpFactory, serviceKey), reqFactoringSettings);
            });

            _registeredApiKeys.Add(serviceKey);
        }
    }

    class ScopedApiContractRegistrar : IApiContractRegistrar
    {
        readonly List<string> _registeredApiKeys = new List<string>();

        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of <see cref="ScopedApiContractRegistrar"/>
        /// </summary>
        public ScopedApiContractRegistrar(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerable<string> GetRegisteredApiKeys()
        {
            return _registeredApiKeys;
        }

        public void RegisterContract<TContract>(string httpClientName)
            where TContract : class
        {
            if (httpClientName == null) throw new ArgumentNullException(nameof(httpClientName));
            RegisterContractCore<TContract>(httpClientName);
        }

        public void RegisterContract<TContract>() where TContract : class
        {
            RegisterContractCore<TContract>(ApiConfigKeyProvider.Provide(typeof(TContract)));
        }

        void RegisterContractCore<TContract>(string serviceKey) where TContract : class
        {
            var validationResult = new ApiContractValidator().Validate(typeof(TContract));

            if (!validationResult.Success)
                throw new ApiContractValidationException(validationResult);

            _services.AddScoped(serviceProvider =>
            {
                var opts = serviceProvider.GetService<IOptions<ApiClientsOptions>>();

                var reqFactoringSettings = opts?.Value != null
                    ? RequestFactoringSettings.CreateFromOptions(opts.Value)
                    : null;

                //var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClientFactory = (IHttpClientFactory)serviceProvider.GetService(typeof(IHttpClientFactory));

                var httpClient = serviceKey != null
                    ? httpClientFactory.CreateClient(serviceKey)
                    : httpClientFactory.CreateClient();

                return ApiProxy<TContract>.Create(new SingleHttpClientProvider(httpClient), reqFactoringSettings);
            });

            _registeredApiKeys.Add(serviceKey);
        }
    }
}
