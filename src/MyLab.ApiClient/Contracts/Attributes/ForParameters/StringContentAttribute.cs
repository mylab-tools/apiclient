using System;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in content with simple string format
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class StringContentAttribute : ContentParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="StringContentAttribute"/>
    /// </summary>
    public StringContentAttribute() : base(new StringHttpContentFactory())
    {
    }
}