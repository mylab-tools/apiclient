using System;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in content as Multipart form
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class MultipartContentAttribute : ContentParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="MultipartContentAttribute"/>
    /// </summary>
    public MultipartContentAttribute() : base(new MultipartFormHttpContentFactory())
    {
    }
}