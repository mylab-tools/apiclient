namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes API request parameter
/// </summary>
interface IRequestParameterDescription
{
    /// <summary>
    /// Position in method arguments
    /// </summary>
    int Position { get; }
}