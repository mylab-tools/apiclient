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

    }

    public class QueryParameterInjector : IInputParameterInjector
    {

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
    }
}