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
        /// <summary>
        /// Response status code
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Server message
        /// </summary>
        public string ServerMessage { get; }

        /// <summary>
        /// Format serverMessage
        /// </summary>
        public static string CreateDescription(HttpStatusCode statusCode, string serverMessage)
            => $"{(int) statusCode}({statusCode}). {serverMessage ?? "[no message]"}.";

        /// <summary>
        /// Initializes a new instance of <see cref="ResponseCodeException"/>
        /// </summary>
        public ResponseCodeException(HttpStatusCode statusCode, string serverMessage) 
            : base(CreateDescription(statusCode, serverMessage))
        {
            StatusCode = statusCode;
            ServerMessage = serverMessage;
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

                if (contentStream.CanSeek)
                    contentStream.Seek(0, SeekOrigin.Begin);

                using var rdr = new StreamReader(contentStream);

                var buff = new char[1024];
                var read = await rdr.ReadBlockAsync(buff, 0, 1024);

                return new string(buff, 0, read);
            }
        }
    }
}