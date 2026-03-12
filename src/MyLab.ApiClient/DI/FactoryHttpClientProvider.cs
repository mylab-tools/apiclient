using System;
using System.Net.Http;

namespace MyLab.ApiClient.DI;

class FactoryHttpClientProvider : IHttpClientProvider
{
    readonly IHttpClientFactory _httpClientFactory;
    readonly string _serviceName;

    public FactoryHttpClientProvider(IHttpClientFactory httpClientFactory, string serviceName)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
    }

    public HttpClient Provide()
    {
        return _httpClientFactory.CreateClient(_serviceName);
    }
}