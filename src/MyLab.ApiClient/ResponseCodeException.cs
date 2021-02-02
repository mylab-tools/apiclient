using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when got an unexpected response HTTP code
    /// </summary>
    public class ResponseCodeException : ApiClientException
    {
        public HttpStatusCode StatusCode { get; }

        public static string CreateDescription(HttpStatusCode statusCode, string message)
            => $"{(int) statusCode}({statusCode}). {message ?? "[no message]"}.";

        public ResponseCodeException(HttpStatusCode statusCode, string message) 
            : base(CreateDescription(statusCode, message))
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Creates <see cref="ResponseCodeException"/> based on <see cref="HttpResponseMessage"/>
        /// </summary>
        public static async Task<ResponseCodeException> FromResponseMessage(HttpResponseMessage responseMsg)
        {
            var contentString = await GetMessageFromResponseContentAsync(responseMsg.Content);
            var msg = !string.IsNullOrWhiteSpace(contentString)
                ? contentString
                : responseMsg.ReasonPhrase;

            return new ResponseCodeException(responseMsg.StatusCode, msg);

            async Task<string> GetMessageFromResponseContentAsync(HttpContent responseContent)
            {
                var contentStream = await responseContent.ReadAsStreamAsync();

                using var rdr = new StreamReader(contentStream);

                var buff = new char[1024];
                var read = await rdr.ReadBlockAsync(buff, 0, 1024);

                return new string(buff, 0, read);
            }
        }
    }
}