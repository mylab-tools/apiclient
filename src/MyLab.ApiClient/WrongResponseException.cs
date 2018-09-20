using System;
using System.Net;
using System.Net.Http;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when non-successful response was received
    /// </summary>
    public class WrongResponseException : Exception
    {
        /// <summary>
        /// Gets response status code
        /// </summary>
        public HttpStatusCode StatusCode { get; }
        /// <summary>
        /// Gets request message
        /// </summary>
        public HttpRequestMessage Request { get; }
        /// <summary>
        /// Gets response message
        /// </summary>
        public HttpResponseMessage Response { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WrongResponseException"/>
        /// </summary>
        public WrongResponseException(HttpRequestMessage request, HttpResponseMessage response)
            : base($"Wrong response code was received ({(int)response.StatusCode} {response.StatusCode})")
        {
            StatusCode = response.StatusCode;
            Request = request;
            Response = response;
        }
    }
}
