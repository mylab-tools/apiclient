using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class HttpRequestInvoker : IHttpRequestInvoker
    {
        private readonly HttpOptions _options;

        public HttpRequestInvoker(HttpOptions options)
        {
            _options = options;
        }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            using (var cl = new HttpClient())
            {
                cl.BaseAddress = new Uri(_options.BasePath);

                foreach (var optionsHeader in _options.Headers)
                {
                    cl.DefaultRequestHeaders.Add(optionsHeader.Key, optionsHeader.Value);
                }

                if (_options.Timeout != TimeSpan.Zero)
                {
                    cl.Timeout = _options.Timeout;
                }

                return await cl.SendAsync(requestMessage, cancellationToken);
            }
        }
    }
}