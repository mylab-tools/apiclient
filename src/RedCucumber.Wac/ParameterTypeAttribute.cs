using System;

namespace RedCucumber.Wac
{
    /// <summary>
    /// Determines web api parameter type
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets parameter type
        /// </summary>
        public WebApiParameterType ParameterType { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ParameterTypeAttribute"/>
        /// </summary>
        public ParameterTypeAttribute(WebApiParameterType parameterType)
        {
            ParameterType = parameterType;
        }
    }

    /// <summary>
    /// Determines payload http request parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PayloadAttribute : ParameterTypeAttribute
    {
        /// <inheritdoc />
        public PayloadAttribute()
            : base(WebApiParameterType.Payload)
        {
        }
    }

    /// <summary>
    /// Determines payload http request form item parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FormItemAttribute : ParameterTypeAttribute
    {
        /// <inheritdoc />
        public FormItemAttribute() 
            : base(WebApiParameterType.FormItem)
        {
        }
    }

    /// <summary>
    /// Determines parameter as a GET-parameter in URL 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class GetParamAttribute : ParameterTypeAttribute
    {
        /// <inheritdoc />
        public GetParamAttribute() 
            :base(WebApiParameterType.Get)
        {
        }
    }
}