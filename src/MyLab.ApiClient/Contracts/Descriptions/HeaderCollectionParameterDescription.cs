using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.Tools;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes parameter which is a collection of the headers, and it will be added into headers
/// </summary>
class HeaderCollectionParameterDescription : IRequestParameterDescription
{
    public int Position { get; }
    /// <summary>
    /// Request factoring settings
    /// </summary>
    public RequestFactoringSettings? Settings { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderCollectionParameterDescription"/>
    /// </summary>
    public HeaderCollectionParameterDescription(int position)
    {
        Position = position;
    }

    public void Apply(HttpRequestMessage request, object? value)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        if (value is IEnumerable<KeyValuePair<string, object>> headerMap)
        {
            foreach (var kv in headerMap)
            {
                var strVal = ObjectToStringConverter.ToString(kv.Value);
                request.Headers.Add(kv.Key, strVal);
            }
        }
        else
            throw new ArgumentException("Value must be a IEnumerable<KeyValuePair<string, object>>", nameof(value));
    }
}