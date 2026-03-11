using System;
using System.Net.Http;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Models;

/// <summary>
/// Represent parameter which will be used to create request content
/// </summary>
class ContentParameterModel : IRequestParameterModel
{
    public int Position { get; }
    public IHttpContentFactory ContentFactory { get; }
    
    public RequestFactoringSettings? Settings { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ContentParameterModel"/>
    /// </summary>
    public ContentParameterModel(int position, IHttpContentFactory contentFactory)
    {
        Position = position;
        ContentFactory = contentFactory;
    }

    public void Apply(HttpRequestMessage request, object? value)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        request.Content = ContentFactory.Create(value, Settings);
    }
}