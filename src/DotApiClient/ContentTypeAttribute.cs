using System;

namespace DotAspectClient
{
    /// <summary>
    /// Declares request content type
    /// </summary>
    public class ContentTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets request content type
        /// </summary>
        public ContentType ContentType { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ContentTypeAttribute"/>
        /// </summary>
        public ContentTypeAttribute(ContentType contentType)
        {
            ContentType = contentType;
        }
    }
}