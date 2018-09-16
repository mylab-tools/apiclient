using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Determines a WEB API abstraction
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ApiAttribute : Attribute
    {
        /// <summary>
        /// Related resource path
        /// </summary>
        public string RelPath { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiAttribute"/>
        /// </summary>
        public ApiAttribute()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiAttribute"/>
        /// </summary>
        public ApiAttribute(string relPath)
        {
            RelPath = relPath;
        }
    }
}
