using System;
using System.Net.Http;

namespace MyLab.ApiClient.Contracts.Attributes;

/// <summary>
/// Determines that the method corresponds to the PUT HTTP method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PutAttribute : ApiMethodAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PutAttribute"/>
    /// </summary>
    public PutAttribute(string? url = null)
        : base(url, HttpMethod.Put)
    {
            
    }
}