using System.Net.Http;
using System.Threading.Tasks;

namespace DotApiClient
{
    /// <summary>
    /// Processes web api requests
    /// </summary>
    public interface IWebApiRequestProcessor
    {
        /// <summary>
        /// Processes web api requests
        /// </summary>
        Task<HttpResponseMessage> ProcessRequest(HttpRequestMessage message);
    }

    /// <summary>
    /// Default request processor
    /// </summary>
    public class DefaultWebApiRequestProcessor : IWebApiRequestProcessor
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultWebApiRequestProcessor"/>
        /// </summary>
        public DefaultWebApiRequestProcessor()
            :this(CreateHttpClient())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultWebApiRequestProcessor"/>
        /// </summary>
        public DefaultWebApiRequestProcessor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> ProcessRequest(HttpRequestMessage message)
        {
            return _httpClient.SendAsync(message);
        }

        private static HttpClient CreateHttpClient()
        {
            var mh = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            return new HttpClient(mh);
        }
    }
}
