using System;

namespace MyLab.ApiClient;

/// <summary>Legacy GET method attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForMethod.GetAttribute.")]
[AttributeUsage(AttributeTargets.Method)]
public class GetAttribute : Contracts.Attributes.ForMethod.GetAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForMethod.GetAttribute(string?)"/>
    public GetAttribute(string? url = null) : base(url)
    {
    }
}

/// <summary>Legacy POST method attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForMethod.PostAttribute.")]
[AttributeUsage(AttributeTargets.Method)]
public class PostAttribute : Contracts.Attributes.ForMethod.PostAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForMethod.PostAttribute(string?)"/>
    public PostAttribute(string? url = null) : base(url)
    {
    }
}

/// <summary>Legacy PUT method attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForMethod.PutAttribute.")]
[AttributeUsage(AttributeTargets.Method)]
public class PutAttribute : Contracts.Attributes.ForMethod.PutAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForMethod.PutAttribute(string?)"/>
    public PutAttribute(string? url = null) : base(url)
    {
    }
}

/// <summary>Legacy PATCH method attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForMethod.PatchAttribute.")]
[AttributeUsage(AttributeTargets.Method)]
public class PatchAttribute : Contracts.Attributes.ForMethod.PatchAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForMethod.PatchAttribute(string?)"/>
    public PatchAttribute(string? url = null) : base(url)
    {
    }
}

/// <summary>Legacy DELETE method attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForMethod.DeleteAttribute.")]
[AttributeUsage(AttributeTargets.Method)]
public class DeleteAttribute : Contracts.Attributes.ForMethod.DeleteAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForMethod.DeleteAttribute(string?)"/>
    public DeleteAttribute(string? url = null) : base(url)
    {
    }
}

/// <summary>Legacy HEAD method attribute from MyLab.ApiClient 3.x.</summary>
[Obsolete("Use MyLab.ApiClient.Contracts.Attributes.ForMethod.HeadAttribute.")]
[AttributeUsage(AttributeTargets.Method)]
public class HeadAttribute : Contracts.Attributes.ForMethod.HeadAttribute
{
    /// <inheritdoc cref="Contracts.Attributes.ForMethod.HeadAttribute(string?)"/>
    public HeadAttribute(string? url = null) : base(url)
    {
    }
}
