using System;

namespace DotAspectClient
{
    /// <summary>
    /// Determines REST action
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RestActionAttribute : EnpointBaseAttribute
    {
        /// <summary>
        /// Gets request http method
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="RestActionAttribute"/>
        /// </summary>
        public RestActionAttribute(HttpMethod method)
        {
            Method = method;
        }
    }
}