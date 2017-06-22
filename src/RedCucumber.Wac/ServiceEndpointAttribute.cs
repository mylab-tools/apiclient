using System;

namespace RedCucumber.Wac
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
        public string Path { get; }

        /// <summary>
        /// Gets endpoint http method
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceEndpointAttribute"/>
        /// </summary>
        /// <param name="path">Endpoint path relates to base path and subpath.</param>
        /// <param name="method">http method</param>
        public ServiceEndpointAttribute(HttpMethod method, string path)
        {
            Path = path;
            Method = method;
        }
    }
}
