using System;

namespace DotAspectClient
{
    /// <summary>
    /// Describes REST api service
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RestApiAttribute : WebApiServiceAttribute
    {
    }
}
