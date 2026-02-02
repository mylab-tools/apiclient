using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes parameter which will be used to create request content
/// </summary>
class ContentParameterDescription : IRequestParameterDescription
{
    public int Position { get; }
    public IHttpContentFactory ContentFactory { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ContentParameterDescription"/>
    /// </summary>
    public ContentParameterDescription(int position, IHttpContentFactory contentFactory)
    {
        Position = position;
        ContentFactory = contentFactory;
    }
}