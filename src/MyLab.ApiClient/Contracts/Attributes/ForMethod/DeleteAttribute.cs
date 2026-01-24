using System;
using System.Net.Http;

namespace MyLab.ApiClient.Contracts.Attributes.ForMethod;

/// <summary>
/// Determines that the method corresponds to the DELETE HTTP method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class DeleteAttribute : ApiMethodAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="DeleteAttribute"/>
    /// </summary>
    public DeleteAttribute(string? url = null)
        : base(url, HttpMethod.Delete)
    {
            
    }
}