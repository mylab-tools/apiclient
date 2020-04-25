using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when api contract validation was not passed
    /// </summary>
    public class ApiContractValidationException : ApiContractException
    {
        public ApiContractValidationResult ValidationResult { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiContractValidationException"/>
        /// </summary>
        public ApiContractValidationException(ApiContractValidationResult validationResult) 
            : base("Invalid service api contract")
        {
            ValidationResult = validationResult;
        }
    }
}