namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes parameter which is a collection of the headers, and it will be added into headers
/// </summary>
class HeaderCollectionParameterDescription : IRequestParameterDescription
{
    public int Position { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderCollectionParameterDescription"/>
    /// </summary>
    public HeaderCollectionParameterDescription(int position)
    {
        Position = position;
    }
}