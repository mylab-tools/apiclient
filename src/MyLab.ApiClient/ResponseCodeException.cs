using System;
using System.Net;

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
    }
}