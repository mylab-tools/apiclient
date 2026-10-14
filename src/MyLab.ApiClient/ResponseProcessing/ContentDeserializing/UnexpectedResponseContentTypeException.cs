using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

/// <summary>
/// Throws when response content has unexpected media type
/// </summary>
public class UnexpectedResponseContentTypeException : Exception
{
    /// <summary>
    /// Gets the actual content type of the response content that caused the exception.
    /// </summary>
    public string ActualContentType { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="UnexpectedResponseContentTypeException"/>
    /// </summary>
    public UnexpectedResponseContentTypeException(string actualContentType)
        : base($"The response has unexpected media type: '{actualContentType}'")
    {
        ActualContentType = actualContentType;
    }
}