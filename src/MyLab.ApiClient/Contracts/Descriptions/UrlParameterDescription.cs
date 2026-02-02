using MyLab.ApiClient.RequestFactoring.UrlModifying;

namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes parameter which will modify a URL
/// </summary>
class UrlParameterDescription : IRequestParameterDescription
{
    public int Position { get; }
    public string Name { get; }
    public IUrlModifier Modifier { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="UrlParameterDescription"/>
    /// </summary>
    public UrlParameterDescription(int position, string name, IUrlModifier modifier)
    {
        Position = position;
        Name = name;
        Modifier = modifier;
    }
}