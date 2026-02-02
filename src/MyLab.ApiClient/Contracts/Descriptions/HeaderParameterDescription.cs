namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes parameter which will be added into headers
/// </summary>
class HeaderParameterDescription : IRequestParameterDescription
{
    public int Position { get; }
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderParameterDescription"/>
    /// </summary>
    public HeaderParameterDescription(int position, string name)
    {
        Position = position;
        Name = name;
    }
}