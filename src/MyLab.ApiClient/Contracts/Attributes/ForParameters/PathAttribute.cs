using System;
using MyLab.ApiClient.RequestFactoring.UrlModifying;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in URL path
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class PathAttribute : UrlParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PathAttribute"/>
    /// </summary>
    public PathAttribute(string? name = null) : base(name, new UrlPathInjector())
    {
    }
}