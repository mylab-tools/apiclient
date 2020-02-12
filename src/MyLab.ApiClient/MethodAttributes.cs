using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// The base class for method attributes
    /// </summary>
    public class ApiMethodAttribute : ApiMarkupAttribute
    {
        /// <summary>
        /// Related end-point url
        /// </summary>
        public string Url { get; }
        
        /// <summary>
        /// HTTP method
        /// </summary>
        public string HttpMethod { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiMethodAttribute"/>
        /// </summary>
        protected ApiMethodAttribute(string url, string httpMethod)
        {
            Url = url;
            HttpMethod = httpMethod;
        }
    }

    /// <summary>
    /// GET HTTP method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GetAttribute"/>
        /// </summary>
        public GetAttribute(string url)
            :base(url, "GET")
        {
        }
    }

    /// <summary>
    /// HEAD HTTP method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HeadAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HeadAttribute"/>
        /// </summary>
        public HeadAttribute(string url)
            : base(url, "HEAD")
        {
        }
    }

    /// <summary>
    /// POST HTTP method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PostAttribute"/>
        /// </summary>
        public PostAttribute(string url)
            : base(url, "POST")
        {
        }
    }

    /// <summary>
    /// PUT HTTP method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PutAttribute"/>
        /// </summary>
        public PutAttribute(string url)
            : base(url, "PUT")
        {
        }
    }

    /// <summary>
    /// DELETE HTTP method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DeleteAttribute"/>
        /// </summary>
        public DeleteAttribute(string url)
            : base(url, "DELETE")
        {
        }
    }

    /// <summary>
    /// PATCH HTTP method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchAttribute : ApiMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PatchAttribute"/>
        /// </summary>
        public PatchAttribute(string url)
            : base(url, "PATCH")
        {
        }
    }
}