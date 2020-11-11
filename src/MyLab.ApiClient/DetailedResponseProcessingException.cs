using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Occurs in Details provided method when response processing error
    /// </summary>
    public class DetailedResponseProcessingException<TRes> : ResponseProcessingException
    {
        /// <summary>
        /// Gets or sets call details
        /// </summary>
        public CallDetails<TRes> CallDetails { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DetailedResponseProcessingException{TRes}"/>
        /// </summary>
        public DetailedResponseProcessingException(CallDetails<TRes> details, Exception baseException) : base(baseException)
        {
            CallDetails = details;
        }
    }
}