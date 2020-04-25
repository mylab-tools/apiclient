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
        /// Initializes a new instance of <see cref="ApiClient"/>
        /// </summary>
        public ApiClient(IHttpClientProvider httpClientProvider)
        {
            _reqFactory = new ApiRequestFactory<TContract>(httpClientProvider);
        }

        public ApiRequest<string> Call(Expression<Func<TContract, Task>> serviceCallExpr)
        {
            return _reqFactory.Create(serviceCallExpr);
        }

        public ApiRequest<TRes> Call<TRes>(Expression<Func<TContract, Task<TRes>>> serviceCallExpr)
        {
            return _reqFactory.Create(serviceCallExpr);
        }
    }
}