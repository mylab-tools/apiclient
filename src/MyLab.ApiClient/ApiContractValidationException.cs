using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when api contract validation was not passed
    /// </summary>
    public class ApiContractValidationException : ApiContractException
    {
        public ReadOnlyCollection<MarkupValidationIssuer> ValidationIssues { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiContractValidationException"/>
        /// </summary>
        public ApiContractValidationException(IEnumerable<MarkupValidationIssuer> validationIssuer) 
            : base("Invalid service api contract")
        {
            ValidationIssues = new ReadOnlyCollection<MarkupValidationIssuer>(
                new List<MarkupValidationIssuer>(validationIssuer));
        }
    }
}