using System;

namespace MyLab.ApiClient.Contracts.Attributes;

/// <summary>
/// Obsolete
/// </summary>
[Obsolete("Use ApiContract attribute instead", true)]
[AttributeUsage(AttributeTargets.Interface)]
public class ApiAttribute : Attribute
{
}