using System;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in content as Url Encoded Form
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class FormContentAttribute : ContentParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="FormContentAttribute"/>
    /// </summary>
    public FormContentAttribute() : base(new UrlFormHttpContentFactory())
    {
    }
}