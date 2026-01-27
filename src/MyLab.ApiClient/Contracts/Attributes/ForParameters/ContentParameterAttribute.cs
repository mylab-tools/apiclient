using System;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines api request parameter which place in content
/// </summary>
public class ContentParameterAttribute : ApiParameterAttribute
{
    /// <summary>
    /// Gets content factory
    /// </summary>
    public IHttpContentFactory HttpContentFactory { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ContentParameterAttribute"/>
    /// </summary>
    protected ContentParameterAttribute(IHttpContentFactory httpContentFactory)
    {
        HttpContentFactory = httpContentFactory;
    }
}