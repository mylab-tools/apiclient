using System;
using System.Net;
using System.Net.Http;

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
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiMethodAttribute"/>
        /// </summary>
        protected ApiMethodAttribute(string url, HttpMethod httpMethod)
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
        public GetAttribute(string url = null)
            :base(url, HttpMethod.Get)
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
        public HeadAttribute(string url = null)
            : base(url, HttpMethod.Head)
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
        public PostAttribute(string url = null)
            : base(url, HttpMethod.Post)
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
        public PutAttribute(string url = null)
            : base(url, HttpMethod.Put)
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
        public DeleteAttribute(string url = null)
            : base(url, HttpMethod.Delete)
        {
        }
    }

    /// <summary>
    /// Determines expected response HTTP code
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExpectedCodeAttribute : ApiMarkupAttribute
    {
        /// <summary>
        /// Expected response HTTP code
        /// </summary>
        public HttpStatusCode ExpectedCode { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ExpectedCodeAttribute"/>
        /// </summary>
        public ExpectedCodeAttribute(HttpStatusCode expectedCode)
        {
            ExpectedCode = expectedCode;
        }
    }
}