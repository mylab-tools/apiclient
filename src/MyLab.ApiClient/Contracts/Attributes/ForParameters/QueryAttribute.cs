using System;
using MyLab.ApiClient.RequestFactoring.UrlModifying;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in URL query
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class QueryAttribute : UrlParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="QueryAttribute"/>
    /// </summary>
    public QueryAttribute(string? name = null) : base(name, new UrlQueryInjector())
    {
    }
}