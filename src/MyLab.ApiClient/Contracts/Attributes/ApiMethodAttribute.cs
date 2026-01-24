using System;
using System.Net.Http;

namespace MyLab.ApiClient.Contracts.Attributes;

/// <summary>
/// The base class for method attributes
/// </summary>
public class ApiMethodAttribute : Attribute
{
    /// <summary>
    /// Related end-point url
    /// </summary>
    public string? Url { get; }

    /// <summary>
    /// HTTP method
    /// </summary>
    public HttpMethod HttpMethod { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ApiMethodAttribute"/>
    /// </summary>
    protected ApiMethodAttribute(string? url, HttpMethod httpMethod)
    {
        Url = url;
        HttpMethod = httpMethod;
    }
}