using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Describes WEB API parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ApiParamAttribute : Attribute
    {
        /// <summary>
        /// Gets where a parameter will placing
        /// </summary>
        public ApiParamPlace Place { get; }

        /// <summary>
        /// Overrides parameter name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiParamAttribute"/>
        /// </summary>
        public ApiParamAttribute(ApiParamPlace place)
        {
            Place = place;
        }
    }

    /// <summary>
    /// Determines parameter place
    /// </summary>
    public enum ApiParamPlace
    {
        Undefined,
        /// <summary>
        /// Request body
        /// </summary>
        Body,
        /// <summary>
        /// Part of URL query
        /// </summary>
        Query,
        /// <summary>
        /// Part of URL path
        /// </summary>
        Path,
        /// <summary>
        /// HTTP header
        /// </summary>
        Header,
        /// <summary>
        /// Part of HTTP Form
        /// </summary>
        Form
    }
}
