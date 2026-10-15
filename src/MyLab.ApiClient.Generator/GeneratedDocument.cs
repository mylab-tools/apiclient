namespace MyLab.ApiClient.Generator;

/// <summary>
/// C# sources produced from an OpenAPI document.
/// </summary>
public sealed class GeneratedDocument
{
    /// <summary>
    /// Initializes a new instance of <see cref="GeneratedDocument"/>.
    /// </summary>
    public GeneratedDocument(string @namespace, IReadOnlyList<GeneratedFile> files)
    {
        Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        Files = files ?? throw new ArgumentNullException(nameof(files));
    }

    /// <summary>
    /// Namespace that contains the generated types.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Generated source files.
    /// </summary>
    public IReadOnlyList<GeneratedFile> Files { get; }
}

/// <summary>
/// A single generated C# file.
/// </summary>
public sealed class GeneratedFile
{
    /// <summary>
    /// Initializes a new instance of <see cref="GeneratedFile"/>.
    /// </summary>
    public GeneratedFile(string name, string code)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
    }

    /// <summary>
    /// File name, including the <c>.cs</c> extension.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// File contents.
    /// </summary>
    public string Code { get; }
}
