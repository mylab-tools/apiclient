using System;
using System.Net;

namespace MyLab.ApiClient.Contracts.Attributes.ForMethod
{
    /// <summary>
    /// Defines expected response parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExpectedResponseAttribute : Attribute
    {
        /// <summary>
        /// Gets expected response code
        /// </summary>
        public HttpStatusCode ExpectedCode { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ExpectedResponseAttribute"/>
        /// </summary>
        public ExpectedResponseAttribute(HttpStatusCode expectedCode)
        {
            ExpectedCode = expectedCode;
        }
    }
}
