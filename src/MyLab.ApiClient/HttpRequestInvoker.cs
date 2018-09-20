using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class HttpRequestInvoker : IHttpRequestInvoker
    {
        private readonly IHttpClientProvider _httpClientProvider;

        public HttpRequestInvoker(IHttpClientProvider httpClientProvider)
        {
            _httpClientProvider = httpClientProvider;
        }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var cl = _httpClientProvider.Provide();
            return await cl.SendAsync(requestMessage, cancellationToken);
        }
    }
}