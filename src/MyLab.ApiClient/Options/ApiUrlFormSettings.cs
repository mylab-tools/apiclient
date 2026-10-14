namespace MyLab.ApiClient.Options;

/// <summary>
/// Defines url-encoded-form serialization settings
/// </summary>
public class ApiUrlFormSettings
{
    /// <summary>
    /// Specifies the default behavior for escaping symbols during URL-encoded form serialization.
    /// </summary>
    public const bool DefaultEscapeSymbols = true;

    /// <summary>
    /// Converts symbols to its escaped representation. True by default
    /// </summary>
    public bool EscapeSymbols { get; set; } = DefaultEscapeSymbols;

    /// <summary>
    /// Ignores null fields. False by default.
    /// </summary>
    public bool IgnoreNullValues { get; set; } = false;
}