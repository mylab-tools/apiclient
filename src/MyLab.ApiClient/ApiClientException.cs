using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when some thing wrong with Api client
    /// </summary>
    public class ApiClientException : Exception
    {
        public ApiClientException(string message)
            :base(message)
        {
            
        }
        
        public ApiClientException(string message, Exception innerException)
            :base(message, innerException)
        {
            
        }
    }
}