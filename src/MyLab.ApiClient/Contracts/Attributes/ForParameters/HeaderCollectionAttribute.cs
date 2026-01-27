using System;

namespace MyLab.ApiClient.Contracts.Attributes.ForParameters;

/// <summary>
/// Determines api request parameter which place in header
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class HeaderCollectionAttribute : Attribute;