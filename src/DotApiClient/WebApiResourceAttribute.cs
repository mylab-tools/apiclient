using System;

namespace DotAspectClient
{
    /// <summary>
    /// Determines reource with access through web REST api
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class WebApiResourceAttribute : Attribute
    {
    }
}
