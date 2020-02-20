using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// The base class for input parameter attributes
    /// </summary>
    public class ApiParameterAttribute : ApiMarkupAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiParameterAttribute"/>
        /// </summary>
        protected ApiParameterAttribute()
        {
            
        }
    }

    /// <summary>
    /// Determines api request parameter which place in URL
    /// </summary>
    public class UrlParameterAttribute : ApiParameterAttribute
    {
        public string Name { get; }
        public IUriModifier UrlModifier { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UrlParameterAttribute"/>
        /// </summary>
        protected UrlParameterAttribute(string name, IUriModifier urlModifier)
        {
            Name = name;
            UrlModifier = urlModifier;
        }
    }

    /// <summary>
    /// Determines request parameter which place in URL path
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathAttribute : UrlParameterAttribute
    {
        public PathAttribute(string name = null) : base(name, new UrlPathInjector())
        {
        }
    }

    /// <summary>
    /// Determines request parameter which place in URL query
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class QueryAttribute : UrlParameterAttribute
    {
        public QueryAttribute(string name = null) : base(name, new UrlQueryInjector())
        {
        }
    }

    /// <summary>
    /// Determines api request parameter which place in header
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class HeaderAttribute : ApiParameterAttribute
    {
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderAttribute"/>
        /// </summary>
        public HeaderAttribute(string name = null)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Determines api request parameter which place in content
    /// </summary>
    public class ContentParameterAttribute : ApiParameterAttribute
    {
        public IHttpContentFactory HttpContentFactory { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ContentParameterAttribute"/>
        /// </summary>
        protected ContentParameterAttribute(IHttpContentFactory httpContentFactory)
        {
            HttpContentFactory = httpContentFactory;
        }
    }

    /// <summary>
    /// Determines request parameter which place in content with simple string format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class StringContentAttribute : ContentParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StringContentAttribute"/>
        /// </summary>
        public StringContentAttribute() : base(new StringHttpContentFactory())
        {
        }
    }

    /// <summary>
    /// Determines request parameter which place in content with JSON format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JsonContentAttribute :ContentParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonContentAttribute"/>
        /// </summary>
        public JsonContentAttribute(): base(new JsonHttpContentFactory())
        {
        }
    }

    /// <summary>
    /// Determines request parameter which place in content with XML format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class XmlContentAttribute : ContentParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="XmlContentAttribute"/>
        /// </summary>
        public XmlContentAttribute() : base(new XmlHttpContentFactory())
        {
        }
    }

    /// <summary>
    /// Determines request parameter which place in content with XML format
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FormContentAttribute : ContentParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FormContentAttribute"/>
        /// </summary>
        public FormContentAttribute() : base(new UrlFormHttpContentFactory())
        {
        }
    }
}
