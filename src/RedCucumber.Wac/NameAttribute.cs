using System;

namespace RedCucumber.Wac
{
    /// <summary>
    /// Overrides parameter name
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NameAttribute : Attribute
    {
        /// <summary>
        /// Gets overriden parameter name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="PayloadAttribute"/>
        /// </summary>
        /// <param name="name">overrides parameter name</param>
        public NameAttribute(string name = null)
        {
            Name = name;
        }
    }
}