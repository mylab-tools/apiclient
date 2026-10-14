using System;

namespace MyLab.ApiClient;

/// <summary>Legacy path parameter attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.PathAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class PathAttribute : Contracts.Attributes.ForParameters.PathAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForParameters.PathAttribute(string?)"/>
    public PathAttribute(string? name = null) : base(name)
    {
    }
}

/// <summary>Legacy query parameter attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.QueryAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class QueryAttribute : Contracts.Attributes.ForParameters.QueryAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForParameters.QueryAttribute(string?)"/>
    public QueryAttribute(string? name = null) : base(name)
    {
    }
}

/// <summary>Legacy header parameter attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.HeaderAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class HeaderAttribute : Contracts.Attributes.ForParameters.HeaderAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForParameters.HeaderAttribute(string?)"/>
    public HeaderAttribute(string? name = null) : base(name)
    {
    }
}

/// <summary>Legacy header-collection parameter attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.HeaderCollectionAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class HeaderCollectionAttribute : Contracts.Attributes.ForParameters.HeaderCollectionAttribute
{
}

/// <summary>Legacy JSON content attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.JsonContentAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class JsonContentAttribute : Contracts.Attributes.ForParameters.JsonContentAttribute
{
}

/// <summary>Legacy XML content attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.XmlContentAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class XmlContentAttribute : Contracts.Attributes.ForParameters.XmlContentAttribute
{
}

/// <summary>Legacy string content attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.StringContentAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class StringContentAttribute : Contracts.Attributes.ForParameters.StringContentAttribute
{
}

/// <summary>Legacy binary content attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.BinContentAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class BinContentAttribute : Contracts.Attributes.ForParameters.BinContentAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForParameters.BinContentAttribute()"/>
    public BinContentAttribute()
    {
    }

    /// <inheritdoc cref="Contracts.Attributes.ForParameters.BinContentAttribute(string)"/>
    public BinContentAttribute(string mimeType) : base(mimeType)
    {
    }
}

/// <summary>Legacy URL-form content attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.FormContentAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class FormContentAttribute : Contracts.Attributes.ForParameters.FormContentAttribute
{
}

/// <summary>Legacy multipart content attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForParameters.MultipartContentAttribute.")]
[AttributeUsage(AttributeTargets.Parameter)]
public class MultipartContentAttribute : Contracts.Attributes.ForParameters.MultipartContentAttribute
{
}
