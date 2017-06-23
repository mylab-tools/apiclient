using System;

namespace RedCucumber.Wac
{
    /// <summary>
    /// Throws when error found whern interface analysis 
    /// </summary>
    public class WebApiContractException : WebApiException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WebApiContractException"/>
        /// </summary>
        public WebApiContractException()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiContractException"/>
        /// </summary>
        public WebApiContractException(string description)
            :base(description)
        {
            
        }
    }
}
