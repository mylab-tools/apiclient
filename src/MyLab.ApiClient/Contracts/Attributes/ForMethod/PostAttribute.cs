using System;
using System.Net.Http;

namespace MyLab.ApiClient.Contracts.Attributes.ForMethod;

/// <summary>
/// Determines that the method corresponds to the POST HTTP method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PostAttribute : ApiMethodAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PostAttribute"/>
    /// </summary>
    public PostAttribute(string? url = null)
        : base(url, HttpMethod.Post)
    {
            
    }
}