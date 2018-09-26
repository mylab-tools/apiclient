using System;
using System.Net.Http;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Determines WEB API method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiMethodAttribute : Attribute
    {
        /// <summary>
        /// Gets HTTP method
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Related path
        /// </summary>
        public string RelPath { get; set; }

        public ApiMethodAttribute(HttpMethod httpMethod)
        {
            HttpMethod = httpMethod;
        }
    }

    /// <summary>
    /// Determines API GET method
    /// </summary> 
    public class ApiGetAttribute : ApiMethodAttribute
    {
        public ApiGetAttribute() : base(HttpMethod.Get)
        {
        }
    }

    /// <summary>W
    /// Determines API POST method
    /// </summary>
    public class ApiPostAttribute : ApiMethodAttribute
    {
        public ApiPostAttribute() : base(HttpMethod.Post)
        {
        }
    }

    /// <summary>
    /// Determines API PUT method
    /// </summary>
    public class ApiPutAttribute : ApiMethodAttribute
    {
        public ApiPutAttribute() : base(HttpMethod.Put)
        {
        }
    }

    /// <summary>
    /// Determines API HEAD method
    /// </summary>
    public class ApiHeadAttribute : ApiMethodAttribute
    {
        public ApiHeadAttribute() : base(HttpMethod.Head)
        {
        }
    }

    /// <summary>
    /// Determines API DELETE method
    /// </summary>
    public class ApiDeleteAttribute : ApiMethodAttribute
    {
        public ApiDeleteAttribute() : base(HttpMethod.Delete)
        {
        }
    }
}
