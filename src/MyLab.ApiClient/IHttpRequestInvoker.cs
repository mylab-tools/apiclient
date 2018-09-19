using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    interface IHttpRequestInvoker
    {
        Task<HttpResponseMessage> Send(HttpRequestMessage requestMessage, CancellationToken cancellationToken);
    }
}