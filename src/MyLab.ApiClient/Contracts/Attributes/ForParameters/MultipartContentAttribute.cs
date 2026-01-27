using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using System;
using System.Reflection;

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

    /// <inheritdoc />
    public override void ValidateParameter(ParameterInfo p)
    {
        if (!typeof(IMultipartContentParameter).IsAssignableFrom(p.ParameterType))
            throw new InvalidApiContractException("Multipart form parameter must implement IMultipartContentParameter");
    }
}