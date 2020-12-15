using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Occurs in Details provided method when response processing error
    /// </summary>
    public class DetailedResponseProcessingException<TDetails> : ResponseProcessingException
        where TDetails : CallDetails
    {
        /// <summary>
        /// Gets or sets call details
        /// </summary>
        public TDetails CallDetails { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DetailedResponseProcessingException{TDetails}"/>
        /// </summary>
        public DetailedResponseProcessingException(TDetails details, Exception baseException) : base(baseException)
        {
            CallDetails = details;
        }
    }
}