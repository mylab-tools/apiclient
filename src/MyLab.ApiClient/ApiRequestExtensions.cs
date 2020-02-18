using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains extensions for <see cref="ApiRequest{TRes}"/>
    /// </summary>
    public static class ApiRequestExtensions
    {
        public static Task<TRes> Send<TRes>(this ApiRequest<TRes> req)
        {
            return req.Send(CancellationToken.None);
        }
    }
}