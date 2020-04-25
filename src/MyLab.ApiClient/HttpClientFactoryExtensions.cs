using System;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Extension methods for <see cref="IHttpClientFactory"/>
    /// </summary>
    public static class HttpClientFactoryExtensions
    {
        /// <summary>
        /// Creates api client for specified contract
        /// </summary>
        public static ApiClient<TContract> CreateApiClient<TContract>(this IHttpClientFactory httpClientFactory)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            
            var valRes = new ApiContractValidator
            {
                ContractKeyMustBeSpecified = true
            }.Validate(typeof(TContract));

            if(!valRes.Success)
                throw new ApiContractValidationException(valRes);

            var contractKey = typeof(TContract).GetCustomAttribute<ApiAttribute>().Key;

            return new ApiClient<TContract>(new FactoryHttpClientProvider(httpClientFactory, contractKey));
        }
    }
}