using System.Net.Http;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

/// <summary>
/// Creates <see cref="HttpContent"/>
/// </summary>
public interface IHttpContentFactory
{
    /// <summary>
    /// Creates content object
    /// </summary>
    HttpContent Create(object? source, RequestFactoringSettings? settings);
}