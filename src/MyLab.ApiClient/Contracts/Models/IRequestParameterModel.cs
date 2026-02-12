using System.Net.Http;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Models;

/// <summary>
/// Represent API request parameter
/// </summary>
public interface IRequestParameterModel
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