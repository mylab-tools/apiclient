namespace DotAspectClient
{
    /// <summary>
    /// Throws when response processing error occured
    /// </summary>
    public class ResponseProcessingException : WebApiException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ResponseProcessingException"/>
        /// </summary>
        public ResponseProcessingException()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ResponseProcessingException"/>
        /// </summary>
        public ResponseProcessingException(string message)
            : base(message)
        {
            
        }
    }
}