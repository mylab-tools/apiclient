using System;

namespace MyLab.ApiClient.Contracts.Attributes.ForContract;

/// <summary>
/// Defines metadata for an API contract interface.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public class ApiContractAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the relative URL associated with the API contract.
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// Gets or sets the configuration key associated with the API contract.
    /// </summary>
    public string? ConfigKey { get; set; }
}