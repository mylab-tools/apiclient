using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides abilities to send requests to web API
    /// </summary>
    /// <typeparam name="TContract">API contract</typeparam>
    public class ApiClient<TContract>
    {
        private readonly ApiRequestFactory<TContract> _reqFactory;

        /// <summary>
        /// Contains request factoring settings
        /// </summary>
        public RequestFactoringSettings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClient"/>
        /// </summary>
        public ApiClient(IHttpClientProvider httpClientProvider)
        {
            _reqFactory = new ApiRequestFactory<TContract>(httpClientProvider)
            {
                Settings = Settings
            };
        }

        /// <summary>
        /// Creates API request based on contract method without result
        /// </summary>
        public ApiRequest Request(Expression<Func<TContract, Task>> serviceCallExpr)
        {
            return _reqFactory.Create(serviceCallExpr);
        }

        /// <summary>
        /// Creates API request based on contract method with result
        /// </summary>
        public ApiRequest<TRes> Request<TRes>(Expression<Func<TContract, Task<TRes>>> serviceCallExpr)
        {
            return _reqFactory.Create(serviceCallExpr);
        }
    }
}