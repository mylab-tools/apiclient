using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes API request parameter
/// </summary>
public interface IRequestParameterDescription
{
    /// <summary>
    /// Position in method arguments
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Request factoring settings
    /// </summary>
    public RequestFactoringSettings? Settings { get; set; }

    /// <summary>
    /// Applies the parameter to the specified HTTP request.
    /// </summary>
    void Apply(HttpRequestMessage request, object? value);
}