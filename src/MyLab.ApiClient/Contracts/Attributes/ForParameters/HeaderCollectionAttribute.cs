using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines api request parameter which place in header
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class HeaderCollectionAttribute : ApiParameterAttribute
{
    /// <inheritdoc />
    public override void ValidateParameter(ParameterInfo p)
    {
        if (!typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(p.ParameterType))
        {
            throw new InvalidApiContractException(
                "Header collection parameter must implement IDictionary<string,object>");
        }
    }
}