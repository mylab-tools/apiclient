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

        /// <summary>
        /// Initializes a new instance of <see cref="ApiMethodAttribute"/>
        /// </summary>
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
        /// <summary>
        /// Initializes a new instance of <see cref="ApiGetAttribute"/>
        /// </summary>
        public ApiGetAttribute(string repPath)
            :this()
        {
            RelPath = RelPath;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiGetAttribute"/>
        /// </summary>
        public ApiGetAttribute() : base(HttpMethod.Get)
        {
        }
    }

    /// <summary>W
    /// Determines API POST method
    /// </summary>
    public class ApiPostAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiPostAttribute"/>
        /// </summary>
        public ApiPostAttribute(string repPath)
            :this()
        {
            RelPath = RelPath;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiPostAttribute"/>
        /// </summary>
        public ApiPostAttribute() : base(HttpMethod.Post)
        {
        }
    }

    /// <summary>
    /// Determines API PUT method
    /// </summary>
    public class ApiPutAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiPutAttribute"/>
        /// </summary>
        public ApiPutAttribute(string repPath)
            :this()
        {
            RelPath = RelPath;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiPutAttribute"/>
        /// </summary>
        public ApiPutAttribute() : base(HttpMethod.Put)
        {
        }
    }

    /// <summary>
    /// Determines API HEAD method
    /// </summary>
    public class ApiHeadAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiHeadAttribute"/>
        /// </summary>
        public ApiHeadAttribute(string repPath)
            :this()
        {
            RelPath = RelPath;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiHeadAttribute"/>
        /// </summary>
        public ApiHeadAttribute() : base(HttpMethod.Head)
        {
        }
    }

    /// <summary>
    /// Determines API DELETE method
    /// </summary>
    public class ApiDeleteAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiDeleteAttribute"/>
        /// </summary>
        public ApiDeleteAttribute(string repPath)
            :this()
        {
            RelPath = RelPath;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiDeleteAttribute"/>
        /// </summary>
        public ApiDeleteAttribute() : base(HttpMethod.Delete)
        {
        }
    }
}
