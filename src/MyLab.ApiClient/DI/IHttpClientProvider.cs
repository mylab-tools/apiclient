using System.Net.Http;

namespace MyLab.ApiClient.DI
{
    /// <summary>
    /// Provides a mechanism to supply an instance of <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    public interface IHttpClientProvider
    {
        /// <summary>
        /// Provides an instance of <see cref="HttpClient"/> for making HTTP requests.
        /// </summary>
        HttpClient Provide();
    }
}
