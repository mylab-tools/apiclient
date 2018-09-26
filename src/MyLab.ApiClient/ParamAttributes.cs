using System;

namespace MyLab.ApiClient
{
    public abstract class BaseParamAttribute : Attribute
    {
        /// <summary>
        /// Gets where a parameter will placing
        /// </summary>
        public ApiParamPlace Place { get; }

        /// <summary>
        /// Overrides parameter name
        /// </summary>
        public string Name { get; set; }

        protected BaseParamAttribute(ApiParamPlace place)
        {
            Place = place;
        }
    }

    /// <summary>
    /// Describes Query WEB API parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class QueryParamAttribute : BaseParamAttribute
    {
        public QueryParamAttribute() : base(ApiParamPlace.Query)
        {
        }
    }

    /// <summary>
    /// Describes Path WEB API parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathParamAttribute : BaseParamAttribute
    {
        public PathParamAttribute() : base(ApiParamPlace.Path)
        {
        }
    }

    /// <summary>
    /// Describes Header 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class HeaderAttribute : BaseParamAttribute
    {
        public HeaderAttribute() : base(ApiParamPlace.Header)
        {
        }
    }
}
 