using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Describes request Body
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ReqBodyAttribute : BaseParamAttribute
    {
        /// <summary>
        /// Gets request content MIME-type
        /// </summary>
        public string MimeType { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ReqBodyAttribute"/>
        /// </summary>
        protected ReqBodyAttribute(string mimeType) : base(ApiParamPlace.Body)
        {
            MimeType = mimeType;
        }

        /// <summary>
        /// Validates parameter type
        /// </summary>
        public virtual void Validate(Type parameterType)
        {

        }
    }

    /// <summary>
    /// Determines binary body parameter
    /// </summary>
    public class BinaryBodyAttribute : ReqBodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BinaryBodyAttribute"/>
        /// </summary>
        public BinaryBodyAttribute() : base("application/octet-stream")
        {
        }
        
        /// <inheritdoc />
        public override void Validate(Type parameterType)
        {
            if(parameterType != typeof(byte[]))
                throw new ApiDescriptionException("A byte array only supported for BinaryBody");
        }
    }

    /// <summary>
    /// Determines json body parameter
    /// </summary>
    public class JsonBodyAttribute : ReqBodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonBodyAttribute"/>
        /// </summary>
        public JsonBodyAttribute() : base("application/json")
        {
        }
    }

    /// <summary>
    /// Determines text body parameter
    /// </summary>
    public class TextBodyAttribute : ReqBodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TextBodyAttribute"/>
        /// </summary>
        public TextBodyAttribute() : base("text/plain")
        {
        }
    }

    /// <summary>
    /// Determines XML body parameter
    /// </summary>
    public class XmlBodyAttribute : ReqBodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="XmlBodyAttribute"/>
        /// </summary>
        public XmlBodyAttribute() : base("application/xml")
        {
        }
    }
    
    /// <summary>
    /// Determines FORM body parameter
    /// </summary>
    public class FormBodyAttribute : ReqBodyAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FormBodyAttribute"/>
        /// </summary>
        public FormBodyAttribute() : base("application/x-www-form-urlencoded")
        {
        }
    }
}