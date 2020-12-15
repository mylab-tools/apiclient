using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains extensions for <see cref="ApiRequest{TRes}"/>
    /// </summary>
    public static class ApiRequestExtensions
    {
        /// <summary>
        /// Send request and return serialized response
        /// </summary>
        public static async Task<TRes> GetResultAsync<TRes>(this ApiRequest<TRes> req)
        {
            return await req.GetResultAsync(CancellationToken.None);
        }

        /// <summary>
        /// Send request and return detailed information about operation
        /// </summary>
        public static async Task<CallDetails<TRes>> GetDetailedAsync<TRes>(this ApiRequest<TRes> req)
        {
            return await req.GetDetailedAsync(CancellationToken.None);
        }

        /// <summary>
        /// Send request and return detailed information about operation
        /// </summary>
        public static async Task<CallDetails> GetDetailedAsync(this ApiRequest req)
        {
            return await req.GetDetailedAsync(CancellationToken.None);
        }

        /// <summary>
        /// Send request 
        /// </summary>
        public static async Task CallAsync(this ApiRequest req)
        {
            await req.CallAsync(CancellationToken.None);
        }
    }
}