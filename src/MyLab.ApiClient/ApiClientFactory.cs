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
            var valRes = new ApiContractValidator().Validate(typeof(TContract));

            if (!valRes.Success)
                throw new ApiContractValidationException(valRes);

            var contractKey = ApiConfigKeyProvider.Provide(typeof(TContract));

            return new ApiClient<TContract>(new FactoryHttpClientProvider(_httpClientFactory, contractKey))
            {
                Settings = _opts != null 
                    ? RequestFactoringSettings.CreateFromOptions(_opts)
                    : null
            };
        }
    }
}