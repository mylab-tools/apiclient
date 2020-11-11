using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Occurs in Result provided method when response processing error
    /// </summary>
    public class ResponseProcessingException : Exception
    {

        /// <summary>
        /// Initializes a new instance of <see cref="ResponseProcessingException"/>
        /// </summary>
        public ResponseProcessingException(Exception baseException) : base("Response processing error", baseException)
        {
        }
    }
}
