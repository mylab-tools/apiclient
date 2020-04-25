using System;
using System.Collections.Immutable;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when some thing wrong with Api contact
    /// </summary>
    public class ApiContractException : ApiClientException
    {
        public ApiContractException(string message)
            :base(message)
        {
            
        }
        
        public ApiContractException(string message, Exception innerException)
            :base(message, innerException)
        {
            
        }
    }
}