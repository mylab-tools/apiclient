using System;
using System.Net.Http;

namespace MyLab.ApiClient.DI;

class SingleHttpClientProvider : IHttpClientProvider
{
    readonly HttpClient _client;

    public SingleHttpClientProvider(HttpClient client)
    {
        _client = client;
    }

    public HttpClient Provide()
    {
        return _client;
    }

    public static SingleHttpClientProvider CrateFRomHttpClientFactory
    (
        IHttpClientFactory httpClientFactory,
        string serviceName
    )
    {
        if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
        if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));

        return new SingleHttpClientProvider(httpClientFactory.CreateClient(serviceName));
    }
}