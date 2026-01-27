using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using System;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines request parameter which place in content with binary format
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class BinContentAttribute : ContentParameterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="BinContentAttribute"/>
    /// </summary>
    public BinContentAttribute() : base(new BinaryHttpContentFactory())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BinContentAttribute"/>
    /// </summary>
    public BinContentAttribute(string mimeType) : base(new BinaryHttpContentFactory(mimeType))
    {
    }

    /// <inheritdoc />
    public override void ValidateParameter(ParameterInfo p)
    {
        if (!(p.ParameterType == typeof(byte[])))
            throw new InvalidApiContractException("Only 'byte[]' supported as binary argument");
    }
}