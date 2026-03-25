using System;
using System.Net.Http;

namespace MyLab.ApiClient.DI;

/// <summary>
/// Provides an implementation of <see cref="IHttpClientProvider"/> that uses an <see cref="IHttpClientFactory"/> 
/// to create and configure instances of <see cref="HttpClient"/> based on a specified service name.
/// </summary>
/// <remarks>
/// This class is designed to integrate with dependency injection and simplifies the process of obtaining 
/// pre-configured <see cref="HttpClient"/> instances for making HTTP requests.
/// </remarks>
public class FactoryHttpClientProvider : IHttpClientProvider
{
    readonly IHttpClientFactory _httpClientFactory;
    readonly string _serviceName;

    /// <summary>
    /// Initializes a new instance of <see cref="FactoryHttpClientProvider"/>
    /// </summary>
    public FactoryHttpClientProvider(IHttpClientFactory httpClientFactory, string serviceName)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
    }

    /// <inheritdoc />
    public HttpClient Provide()
    {
        return _httpClientFactory.CreateClient(_serviceName);
    }
}