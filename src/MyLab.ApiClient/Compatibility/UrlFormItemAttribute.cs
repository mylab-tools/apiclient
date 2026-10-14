using System;

namespace MyLab.ApiClient;

/// <summary>Legacy URL-form item attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForData.UrlFormItemAttribute.")]
[AttributeUsage(AttributeTargets.Property)]
public class UrlFormItemAttribute : Contracts.Attributes.ForData.UrlFormItemAttribute
{
}
