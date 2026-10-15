namespace MyLab.ApiClient.Generator;

/// <summary>
/// Options that control C# generation from an OpenAPI document.
/// </summary>
public sealed class GenerationOptions
{
    /// <summary>
    /// Namespace of the generated contracts and data types.
    /// When omitted, the namespace is derived from <c>info.title</c>.
    /// </summary>
    public string? Namespace { get; init; }
}
