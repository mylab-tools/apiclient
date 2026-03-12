using MyLab.ApiClient.DI;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage
{
    class HttpClientRequestProcessor : IRequestProcessor
    {
        public IHttpClientProvider HttpClientProvider { get; }

        public HttpClientRequestProcessor(IHttpClientProvider httpClientProvider)
        {
            HttpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
        }


        public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var httpClient = HttpClientProvider.Provide();

            return await httpClient.SendAsync(request, ct);
        }
    }
}
