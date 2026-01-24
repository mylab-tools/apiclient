using System.Net.Http;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

/// <summary>
/// Specifies input parameter which will be used to create request multipart content
/// </summary>
public interface IMultipartContentParameter
{
    /// <summary>
    /// Implement to add parts to content
    /// </summary>
    void AddParts(MultipartFormDataContent content);
}