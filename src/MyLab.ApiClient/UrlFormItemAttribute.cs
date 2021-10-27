using System;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Defines URL-Form item properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UrlFormItemAttribute : Attribute
    {
        /// <summary>
        /// Specify name to override
        /// </summary>
        public string Name { get; set; }
    }
}
