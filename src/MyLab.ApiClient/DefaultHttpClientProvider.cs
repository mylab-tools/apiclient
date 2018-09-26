using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MyLab.ApiClient
{
    class DefaultHttpClientProvider : IHttpClientProvider, IDisposable
    {
        private readonly Lazy<HttpClient> _httpClient;

        public string BasePath { get; }

        public TimeSpan Timeout { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public DefaultHttpClientProvider(string basePath)
        {
            BasePath = basePath;

            _httpClient = new Lazy<HttpClient>(() =>
            {
                var c = new HttpClient();

                if (Timeout != default(TimeSpan))
                    c.Timeout = Timeout;

                if (Headers != null)
                {
                    foreach (var header in Headers)
                    {
                        c.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                return c;
            });
        }

        public HttpClient Provide()
        {
            return _httpClient.Value;
        }

        public void Dispose()
        {
            if(_httpClient.IsValueCreated)
                _httpClient.Value.Dispose();
        }
    }
}