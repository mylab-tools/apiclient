using System.Net.Http;

namespace DotApiClient
{
    /// <summary>
    /// Occures when non 2xx http statud code recieved
    /// </summary>
    public class WarningHttpStatusCodeException : WebApiException
    {
        /// <summary>
        /// Received Http message
        /// </summary>
        public HttpResponseMessage HttpMessage { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WarningHttpStatusCodeException"/>
        /// </summary>
        public WarningHttpStatusCodeException(HttpResponseMessage message)
            :base(message.StatusCode.ToString())
        {
            HttpMessage = message;
        }
    }
}
