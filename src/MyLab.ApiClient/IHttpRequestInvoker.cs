using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Represent object which send/process HTTP request and return HTTP response
    /// </summary>
    public interface IHttpRequestInvoker
    {
        /// <summary>
        /// Sends HTTP request
        /// </summary>
        Task<HttpResponseMessage> Send(HttpRequestMessage requestMessage, CancellationToken cancellationToken);
    }
}