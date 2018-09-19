using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Describes request Body
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ReqBodyAttribute : BaseParamAttribute
    {
        public string MimeType { get; }

        protected ReqBodyAttribute(string mimeType) : base(ApiParamPlace.Body)
        {
            MimeType = mimeType;
        }

        public virtual void Validate(Type parameterType)
        {

        }
    }

    /// <summary>
    /// Determines binary body parameter
    /// </summary>
    public class BinaryBodyAttribute : ReqBodyAttribute
    {
        public BinaryBodyAttribute() : base("application/octet-stream")
        {
        }

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
        public JsonBodyAttribute() : base("application/json")
        {
        }
    }

    /// <summary>
    /// Determines text body parameter
    /// </summary>
    public class TextBodyAttribute : ReqBodyAttribute
    {
        public TextBodyAttribute() : base("text/plain")
        {
        }
    }

    /// <summary>
    /// Determines XML body parameter
    /// </summary>
    public class XmlBodyAttribute : ReqBodyAttribute
    {
        public XmlBodyAttribute() : base("application/xml")
        {
        }
    }
}