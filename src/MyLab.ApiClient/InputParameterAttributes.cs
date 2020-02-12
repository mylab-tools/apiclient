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
            :base(new PathParameterInjector())
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
            : base(new QueryParameterInjector())
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
            : base(new HeaderParameterInjector(headerName))
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
        public BodyAttribute()
            : base(new BodyParameterInjector())
        {

        }
    }
}
