using System.Net.Http;

namespace DotApiClient
{
    /// <summary>
    /// Occures when non 2xx http statud code recieved
    /// </summary>
    public class WrongHttpStatusCodeException : WebApiException
    {
        /// <summary>
        /// Received Http message
        /// </summary>
        public HttpResponseMessage HttpMessage { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WrongHttpStatusCodeException"/>
        /// </summary>
        public WrongHttpStatusCodeException(HttpResponseMessage message)
            :base(message.StatusCode.ToString())
        {
            HttpMessage = message;
        }
    }
}
