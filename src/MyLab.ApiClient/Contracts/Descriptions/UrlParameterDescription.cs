using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.RequestFactoring.UrlModifying;
using System;
using System.Net.Http;
using System.Reflection;

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
    /// Request factoring settings
    /// </summary>
    public RequestFactoringSettings? Settings { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="UrlParameterDescription"/>
    /// </summary>
    public UrlParameterDescription(int position, string name, IUrlModifier modifier)
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