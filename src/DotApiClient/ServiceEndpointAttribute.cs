using System;

namespace DotApiClient
{
    /// <summary>
    /// Declares web api endpoint
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceEndpointAttribute : EnpointBaseAttribute
    {
        /// <summary>
        /// Gets endpoint path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets endpoint http method
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceEndpointAttribute"/>
        /// </summary>
        /// <param name="method">http method</param>
        public ServiceEndpointAttribute(HttpMethod method)
        {
            Method = method;
        }
    }
}
