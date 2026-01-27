using System;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines api request parameter which place in header
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class HeaderAttribute : Attribute
{
    /// <summary>
    /// Gets overriden header name
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderAttribute"/>
    /// </summary>
    public HeaderAttribute(string? name = null)
    {
        Name = name;
    }
}