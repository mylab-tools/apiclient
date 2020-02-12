using System.Net.Http;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Http client provider
    /// </summary>
    public interface IHttpClientProvider
    {
        /// <summary>
        /// Provides <see cref="HttpClient"/>
        /// </summary>
        HttpClient Provide();
    }
}