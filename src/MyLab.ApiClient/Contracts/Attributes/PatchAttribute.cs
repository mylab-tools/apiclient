using System;
using System.Net.Http;

namespace MyLab.ApiClient.Contracts.Attributes;

/// <summary>
/// Determines that the method corresponds to the PATCH HTTP method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PatchAttribute : ApiMethodAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PatchAttribute"/>
    /// </summary>
    public PatchAttribute(string? url = null)
        : base(url, HttpMethod.Patch)
    {
            
    }
}