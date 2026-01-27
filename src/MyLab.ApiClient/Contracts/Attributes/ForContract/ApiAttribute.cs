using System;

namespace MyLab.ApiClient.Contracts.Attributes.ForContract;

/// <summary>
/// Obsolete
/// </summary>
[Obsolete("Use ApiContract attribute instead", true)]
[AttributeUsage(AttributeTargets.Interface)]
public class ApiAttribute : Attribute
{
    /// <summary>
    /// Base service URL
    /// </summary>
    public string? Url { get; }

    /// <summary>
    /// Determine key to bind with configuration
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ApiAttribute"/>
    /// </summary>
    public ApiAttribute(string? url = null)
    {
        Url = url;
    }
}