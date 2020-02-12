using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Places input parameter into request
    /// </summary>
    public interface IInputParameterInjector
    {

    }

    public class PathParameterInjector : IInputParameterInjector
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PathParameterInjector"/>
        /// </summary>
        public PathParameterInjector()
        {
            
        }
    }

    public class QueryParameterInjector : IInputParameterInjector
    {
        /// <summary>
        /// Initializes a new instance of <see cref="QueryParameterInjector"/>
        /// </summary>
        public QueryParameterInjector()
        {
            
        }
    }

    public class HeaderParameterInjector : IInputParameterInjector
    {
        public string HeaderName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderParameterInjector"/>
        /// </summary>
        public HeaderParameterInjector(string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(headerName));
            HeaderName = headerName;
        }
    }

    public class BodyParameterInjector : IInputParameterInjector
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BodyParameterInjector"/>
        /// </summary>
        public BodyParameterInjector()
        {
            
        }
    }
}