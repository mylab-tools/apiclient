using System;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in content with XML format
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class XmlContentAttribute : ContentParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="XmlContentAttribute"/>
    /// </summary>
    public XmlContentAttribute() : base(new XmlHttpContentFactory())
    {
    }
}