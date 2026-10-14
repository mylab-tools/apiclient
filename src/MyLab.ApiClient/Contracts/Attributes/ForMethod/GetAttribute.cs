using System;
using System.Net.Http;

namespace MyLab.ApiClient.Contracts.Attributes.ForMethod
{
    /// <summary>
    /// Determines that the method corresponds to the GET HTTP method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GetAttribute"/>
        /// </summary>
        public GetAttribute(string? url = null)
            : base(url, HttpMethod.Get)
        {
            
        }
    }
}
