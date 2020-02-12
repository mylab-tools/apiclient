using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when wrong api description extracting
    /// </summary>
    public class ApiDescriptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiDescriptionException"/>
        /// </summary>
        public ApiDescriptionException(string message)
            :base(message)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiDescriptionException"/>
        /// </summary>
        public ApiDescriptionException(string message, Exception baseException)
            :base(message, baseException)
        {

        }
    }
}
