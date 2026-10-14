using System;
using System.Net.Http;

namespace MyLab.ApiClient.Contracts.Attributes.ForMethod;

/// <summary>
/// Determines that the method corresponds to the HEAD HTTP method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class HeadAttribute : ApiMethodAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="HeadAttribute"/>
    /// </summary>
    public HeadAttribute(string? url = null)
        : base(url, HttpMethod.Head)
    {
            
    }
}