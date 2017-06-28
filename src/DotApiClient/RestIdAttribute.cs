using System;

namespace DotApiClient
{
    /// <summary>
    /// Determines parameter which will be used as part of REST action url
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RestIdAttribute : Attribute
    {
    }
}
