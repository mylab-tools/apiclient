using System;
using System.Net.Http;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.RequestFactoring.UrlModifying;

namespace MyLab.ApiClient.Contracts.Models;

/// <summary>
/// Represent parameter which will modify a URL
/// </summary>
class UrlParameterModel : IRequestParameterModel
{
    public int Position { get; }
    public string Name { get; }
    public IUrlModifier Modifier { get; }

    /// <summary>
    /// Request factoring settings
    /// </summary>
    public RequestFactoringSettings? Settings { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="UrlParameterModel"/>
    /// </summary>
    public UrlParameterModel(int position, string name, IUrlModifier modifier)
    {
        Position = position;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
    }

    public void Apply(HttpRequestMessage request, object? value)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        request.RequestUri = Modifier.Modify(request.RequestUri ?? new Uri("/", UriKind.Relative), Name, value);
    }
}