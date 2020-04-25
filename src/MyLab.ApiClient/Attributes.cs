using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// The base class for API markup attributes
    /// </summary>
    public class ApiMarkupAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiMarkupAttribute"/>
        /// </summary>
        protected ApiMarkupAttribute()
        {
            
        }
    }

    /// <summary>
    /// Defines API service 
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ApiAttribute : ApiMarkupAttribute
    {
        /// <summary>
        /// Base service URL
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Determine key to bind with configuration
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiAttribute"/>
        /// </summary>
        public ApiAttribute(string url = null)
        {
            Url = url;
        }
    }
}
