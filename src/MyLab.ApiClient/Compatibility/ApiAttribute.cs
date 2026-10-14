using System;
using MyLab.ApiClient.Contracts.Attributes.ForContract;

namespace MyLab.ApiClient;

/// <summary>
/// Legacy contract attribute from MyLab.ApiClient 3.x.
/// Prefer <see cref="ApiContractAttribute"/> with <see cref="ApiContractAttribute.Binding"/>.
/// </summary>
[Obsolete("Use ApiContractAttribute with Binding instead of Key.")]
[AttributeUsage(AttributeTargets.Interface)]
public class ApiAttribute : ApiContractAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApiAttribute"/>.
    /// </summary>
    public ApiAttribute(string? url = null)
    {
        Url = url;
    }

    /// <summary>
    /// Legacy alias of <see cref="ApiContractAttribute.Binding"/>.
    /// </summary>
    public string? Key
    {
        get => Binding;
        set => Binding = value;
    }
}
