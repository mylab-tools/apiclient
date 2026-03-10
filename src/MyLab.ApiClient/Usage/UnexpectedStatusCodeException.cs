using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyLab.ApiClient.Usage
{
    public class UnexpectedStatusCodeException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code that caused the exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedStatusCodeException"/> class with the specified HTTP status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code that caused the exception.</param>
        public UnexpectedStatusCodeException(HttpStatusCode statusCode) : base("Unexpected status code is encountered")
        {
            StatusCode = statusCode;
        }
    }
}
