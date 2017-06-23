using System.Net.Http;
using System.Threading.Tasks;

namespace RedCucumber.Wac
{
    /// <summary>
    /// Processes web api requests
    /// </summary>
    public interface IWebApiRequestProcessor
    {
        Task<HttpResponseMessage> ProcessRequest(HttpRequestMessage message);
    }

    public class WebApiRequestProcessor : IWebApiRequestProcessor
    {
        private readonly HttpClient _httpClient;

        public WebApiRequestProcessor()
            :this(new HttpClient())
        {
            
        }

        public WebApiRequestProcessor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> ProcessRequest(HttpRequestMessage message)
        {
            return _httpClient.SendAsync(message);
        }
    }
}
