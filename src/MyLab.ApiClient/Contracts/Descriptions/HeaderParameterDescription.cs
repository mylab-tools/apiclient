using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.Tools;
using System;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes parameter which will be added into headers
/// </summary>
class HeaderParameterDescription : IRequestParameterDescription
{
    public int Position { get; }
    public string Name { get; }
    /// <summary>
    /// Request factoring settings
    /// </summary>
    public RequestFactoringSettings? Settings { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderParameterDescription"/>
    /// </summary>
    public HeaderParameterDescription(int position, string name)
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