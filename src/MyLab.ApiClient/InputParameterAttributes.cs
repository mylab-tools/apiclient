using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// The base class for input parameter attributes
    /// </summary>
    public class InputParameterAttribute : ApiMarkupAttribute
    {
        public IInputParameterInjector Injector { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="InputParameterAttribute"/>
        /// </summary>
        protected InputParameterAttribute(IInputParameterInjector injector)
        {
            Injector = injector;
        }
    }

    /// <summary>
    /// Define parameter place in end-point URL path
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathAttribute : InputParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PathAttribute"/>
        /// </summary>
        public PathAttribute()
            :base(new PathParameterInjector(new StringParameterFormatter()))
        {
            
        }
    }

    /// <summary>
    /// Define parameter place in end-point URL query
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class QueryAttribute : InputParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="QueryAttribute"/>
        /// </summary>
        public QueryAttribute()
            : base(new QueryParameterInjector(new UrlFormParameterFormatter()))
        {

        }
    }

    /// <summary>
    /// Define parameter place in request header
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class HeaderAttribute : InputParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HeaderAttribute"/>
        /// </summary>
        public HeaderAttribute(string headerName)
            : this(headerName, new StringParameterFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderAttribute"/>
        /// </summary>
        protected HeaderAttribute(string headerName, IInputParameterFormatter formatter)
            : base(new HeaderParameterInjector(headerName, formatter))
        {
        }
    }

    /// <summary>
    /// Define parameter place in request header with JSON format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JsonHeaderAttribute : HeaderAttribute
    {
        public JsonHeaderAttribute(string headerName) 
            : base(headerName, new JsonParameterFormatter())
        {
        }
    }

    /// <summary>
    /// Define parameter place in request header with XML format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class XmlHeaderAttribute : HeaderAttribute
    {
        public XmlHeaderAttribute(string headerName)
            : base(headerName, new XmlParameterFormatter())
        {
        }
    }

    /// <summary>
    /// Define parameter place in request body
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class BodyAttribute : InputParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BodyAttribute"/>
        /// </summary>
        protected BodyAttribute(IInputParameterFormatter formatter)
            : base(new BodyParameterInjector(formatter))
        {

        }
    }

    /// <summary>
    /// Define parameter place in request body with JSON format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JsonBodyAttribute : BodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AttributeTargets"/>
        /// </summary>
        protected JsonBodyAttribute()
            : base(new JsonParameterFormatter())
        {
        }
    }

    /// <summary>
    /// Define parameter place in request body with XML format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class XmlBodyAttribute : BodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="XmlBodyAttribute"/>
        /// </summary>
        protected XmlBodyAttribute()
            : base(new XmlParameterFormatter())
        {
        }
    }

    /// <summary>
    /// Define parameter place in request body with XML format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class StringBodyAttribute : BodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StringBodyAttribute"/>
        /// </summary>
        protected StringBodyAttribute()
            : base(new StringParameterFormatter())
        {
        }
    }
}
