using System;
using System.Net.Http;

namespace MyLab.ApiClient.DI;

/// <summary>
/// Provides a single instance of <see cref="HttpClient"/> for making HTTP requests.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IHttpClientProvider"/> interface and ensures that the same
/// <see cref="HttpClient"/> instance is reused for all requests.
/// </remarks>
public class SingleHttpClientProvider : IHttpClientProvider
{
    readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="SingleHttpClientProvider"/>
    /// </summary>
    public SingleHttpClientProvider(HttpClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public HttpClient Provide()
    {
        return _client;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SingleHttpClientProvider"/> using the specified
    /// <see cref="IHttpClientFactory"/> and service name.
    /// </summary>
    /// <param name="httpClientFactory">
    /// The <see cref="IHttpClientFactory"/> used to create the <see cref="HttpClient"/> instance.
    /// </param>
    /// <param name="serviceName">
    /// The name of the service for which the <see cref="HttpClient"/> is created.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="SingleHttpClientProvider"/> configured with the specified
    /// <see cref="HttpClient"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="httpClientFactory"/> or <paramref name="serviceName"/> is <c>null</c>.
    /// </exception>
    public static SingleHttpClientProvider CrateFromHttpClientFactory
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