using System;
using System.Net.Http;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.Contracts.Models;

/// <summary>
/// Represent parameter which will be added into headers
/// </summary>
class HeaderParameterModel : IRequestParameterModel
{
    public int Position { get; }
    public string Name { get; }
    /// <summary>
    /// Request factoring settings
    /// </summary>
    public RequestFactoringSettings? Settings { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderParameterModel"/>
    /// </summary>
    public HeaderParameterModel(int position, string name)
    {
        Position = position;
        Name = name;
    }

    public void Apply(HttpRequestMessage request, object? value)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (value == null) throw new ArgumentNullException(nameof(value));

        var strVal = ObjectToStringConverter.ToString(value);

        request.Headers.Add(Name, strVal);
    }
}