using System;

namespace MyLab.ApiClient.RequestFactoring.UrlModifying
{
    /// <summary>
    /// Modifies a URI
    /// </summary>
    public interface IUrlModifier
    {
        /// <summary>
        /// Modifies a URI
        /// </summary>
        Uri Modify(Uri origin, string paramName, object? value);
    }
}
