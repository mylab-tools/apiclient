using System;

namespace DotAspectClient
{
    /// <summary>
    /// Declares web api contract interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class WebApiAttribute : WebApiServiceAttribute
    {
    }
}
