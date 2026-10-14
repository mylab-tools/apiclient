namespace MyLab.ApiClient.Options;

/// <summary>
/// Defines JSON serialization settings
/// </summary>
public class ApiJsonSettings
{
    /// <summary>
    /// Ignores null fields. True by default.
    /// </summary>
    public bool IgnoreNullValues { get; set; } = true;
}