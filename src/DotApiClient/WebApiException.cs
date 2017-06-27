using System;

namespace DotApiClient
{
    /// <summary>
    /// THrows when any wb api error occured
    /// </summary>
    public class WebApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WebApiException"/>
        /// </summary>
        public WebApiException()
        {
            
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="WebApiException"/>
        /// </summary>
        public WebApiException(string message)
            :base(message)
        {
            
        }
    }
}
