using System;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters
{
    /// <summary>
    /// The base class for api client parameter attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ApiParameterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiParameterAttribute"/>
        /// </summary>
        protected ApiParameterAttribute()
        {
            
        }
        
        /// <summary>
        /// Override to validate parameter
        /// </summary>
        public virtual void ValidateParameter(ParameterInfo p)
        {
        }
    }
}
