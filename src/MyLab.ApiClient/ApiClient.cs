using System;
using System.Linq.Expressions;
using System.Net.Http;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides abilities to send requests to web API
    /// </summary>
    /// <typeparam name="TContract">API contract</typeparam>
    public class ApiClient<TContract>
    {
        private readonly ApiRequestFactory<TContract> _reqFactory;

        private ApiClient(ServiceDescription description, HttpClient httpClient)
        {
            _reqFactory = new ApiRequestFactory<TContract>(description, httpClient);
        }

        [Obsolete]
        /// <summary>
        /// Creates API client based on http client factory with specified service name
        /// </summary>
        public static ApiClient<TContract> Create(IHttpClientProvider httpClientProvider)

        {
            return new ApiClient<TContract>(
                ServiceDescription.Create(typeof(TContract)),
                httpClientProvider.Provide());
        }

        /// <summary>
        /// Creates API client based on http client factory with specified service name
        /// </summary>
        public static ApiClient<TContract> Create(HttpClient httpClient)
        {
            return new ApiClient<TContract>(
                ServiceDescription.Create(typeof(TContract)),
                httpClient);
        }

        public ApiRequest<string> Call(Expression<Action<TContract>> serviceCallExpr)
        {
            return _reqFactory.Create(serviceCallExpr);
        }

        public ApiRequest<TRes> Call<TRes>(Expression<Func<TContract, TRes>> serviceCallExpr)
        {
            return _reqFactory.Create<TRes>(serviceCallExpr);
        }
    }
}