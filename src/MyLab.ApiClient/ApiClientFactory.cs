using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace MyLab.ApiClient
{
    class ApiClientFactory : IApiClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiClientsOptions _opts;

        public ApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<ApiClientsOptions> opts = null)
            :this(httpClientFactory, opts?.Value)
        {
        }

        public ApiClientFactory(IHttpClientFactory httpClientFactory, ApiClientsOptions opts)
        {
            _httpClientFactory = httpClientFactory;
            _opts = opts;
        }

        public ApiClient<TContract> CreateApiClient<TContract>()
        {
            var valRes = new ApiContractValidator
            {
                ContractKeyMustBeSpecified = true
            }.Validate(typeof(TContract));

            if (!valRes.Success)
                throw new ApiContractValidationException(valRes);

            var contractKey = typeof(TContract).GetCustomAttribute<ApiAttribute>()?.Key;

            if (contractKey == null)
                throw new InvalidOperationException("Api contract key not specified");

            return new ApiClient<TContract>(new FactoryHttpClientProvider(_httpClientFactory, contractKey))
            {
                Settings = _opts != null 
                    ? RequestFactoringSettings.CreateFromOptions(_opts)
                    : null
            };
        }
    }
}