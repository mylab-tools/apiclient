namespace MyLab.ApiClient.Options;

/// <summary>
/// Defines url-encoded-form serialization settings
/// </summary>
public class ApiUrlFormSettings
{
    /// <summary>
    /// Converts symbols to its escaped representation. True by default
    /// </summary>
    public bool EscapeSymbols { get; set; } = true;

    /// <summary>
    /// Ignores null fields. False by default.
    /// </summary>
    public bool IgnoreNullValues { get; set; } = false;
}