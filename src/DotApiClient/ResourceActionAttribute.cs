using System;

namespace DotAspectClient
{
    /// <summary>
    /// Determines resource action
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ResourceActionAttribute : EnpointBaseAttribute
    {
        /// <summary>
        /// Gets request http method
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ResourceActionAttribute"/>
        /// </summary>
        public ResourceActionAttribute(HttpMethod method)
        {
            Method = method;
        }
    }
}