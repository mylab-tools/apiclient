using MyLab.ApiClient.DI;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage
{
    /// <summary>
    /// Processes HTTP requests using an <see cref="IHttpClientProvider"/> to obtain an <see cref="HttpClient"/>.
    /// </summary>
    /// <remarks>
    /// This class implements the <see cref="IRequestProcessor"/> interface and is responsible for sending HTTP requests
    /// using the provided <see cref="HttpClient"/> instance.
    /// </remarks>
    public class HttpClientRequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// Gets the <see cref="IHttpClientProvider"/> instance used to supply <see cref="HttpClient"/> objects
        /// for processing HTTP requests.
        /// </summary>
        /// <value>
        /// An implementation of <see cref="IHttpClientProvider"/> that provides <see cref="HttpClient"/> instances.
        /// </value>
        /// <remarks>
        /// This property is initialized through the constructor and is used internally to obtain
        /// <see cref="HttpClient"/> instances for sending HTTP requests.
        /// </remarks>
        public IHttpClientProvider HttpClientProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpClientRequestProcessor"/>
        /// </summary>
        public HttpClientRequestProcessor(IHttpClientProvider httpClientProvider)
        {
            HttpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var httpClient = HttpClientProvider.Provide();

            return await httpClient.SendAsync(request, ct);
        }
    }
}
