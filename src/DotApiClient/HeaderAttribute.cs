using System;

namespace DotAspectClient
{
    /// <summary>
    /// Determines header parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class HeaderAttribute : Attribute
    {
        /// <summary>
        /// Gets header header name
        /// </summary>
        public string HeaderName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderAttribute"/>
        /// </summary>
        public HeaderAttribute(string headerName)
        {
            HeaderName = headerName;
        }
    }
}
