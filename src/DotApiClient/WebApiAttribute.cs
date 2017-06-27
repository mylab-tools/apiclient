using System;

namespace DotApiClient
{
    /// <summary>
    /// Declares web api contract interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class WebApiAttribute : WebApiServiceAttribute
    {
    }
}
