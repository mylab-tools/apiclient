using MyLab.ApiClient.RequestFactoring.UrlModifying;
using System;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters
{
    /// <summary>
    /// Determines api request parameter which place in URL
    /// </summary>
    public class UrlParameterAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// Gets overriden parameter name
        /// </summary>
        public string? Name { get; }
        
        /// <summary>
        /// Gets URL modifier
        /// </summary>
        public IUrlModifier UrlModifier { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UrlParameterAttribute"/>
        /// </summary>
        protected UrlParameterAttribute(string? name, IUrlModifier urlModifier)
        {
            Name = name;
            UrlModifier = urlModifier;
        }
    }
}
