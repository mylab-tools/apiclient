using System;

namespace DotApiClient
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class WebApiServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets relative path of service
        /// </summary>
        public string RelPath { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiServiceAttribute"/>
        /// </summary>
        protected WebApiServiceAttribute()
        {
            
        }
    }
}