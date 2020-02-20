using System.Net;
using System.Net.Http;
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
        public static async Task<TRes> GetResult<TRes>(this ApiRequest<TRes> req)
        {
            return await req.GetResult(CancellationToken.None);
        }

        /// <summary>
        /// Send request and return detailed information about operation
        /// </summary>
        public static async Task<CallDetails<TRes>> GetDetailed<TRes>(this ApiRequest<TRes> req)
        {
            return await req.GetDetailed(CancellationToken.None);
        }
    }

    /// <summary>
    /// Contains detailed service call information
    /// </summary>
    public class CallDetails<T>
    {
        /// <summary>
        /// Expected response content
        /// </summary>
        public T ResponseContent { get; set; }
        /// <summary>
        /// HTTP status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Gets true if status code is unexpected
        /// </summary>
        public bool IsUnexpectedStatusCode { get; set; }
        /// <summary>
        /// Text request dump
        /// </summary>
        public string RequestDump { get; set; }
        /// <summary>
        /// Text response dump
        /// </summary>
        public string ResponseDump { get; set; }
        /// <summary>
        /// Response object
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; set; }
        /// <summary>
        /// Request object
        /// </summary>
        public HttpRequestMessage RequestMessage { get; set; }
    }
}