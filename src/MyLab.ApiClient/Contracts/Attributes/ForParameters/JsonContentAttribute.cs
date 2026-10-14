using System;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in content with JSON format
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class JsonContentAttribute : ContentParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="JsonContentAttribute"/>
    /// </summary>
    public JsonContentAttribute() : base(new JsonHttpContentFactory())
    {
    }
}