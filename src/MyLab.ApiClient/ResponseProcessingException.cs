using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Error when processing response
    /// </summary>
    public class ResponseProcessingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ResponseProcessingException"/>
        /// </summary>
        public ResponseProcessingException(string message)
            :base(message)
        {
            
        }
    }
}
