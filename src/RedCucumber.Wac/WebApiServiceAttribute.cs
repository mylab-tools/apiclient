using System;

namespace RedCucumber.Wac
{
    /// <summary>
    /// Declares web api contract interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class WebApiServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets sub path relates to base path
        /// </summary>
        public string SubPath { get; }

        /// <summary>
        /// Creates a new instance of <see cref="WebApiServiceAttribute"/>
        /// </summary>
        /// <param name="subPath">Sub path relates to base path</param>
        public WebApiServiceAttribute(string subPath = null)
        {
            SubPath = subPath;
        }
    }
}
