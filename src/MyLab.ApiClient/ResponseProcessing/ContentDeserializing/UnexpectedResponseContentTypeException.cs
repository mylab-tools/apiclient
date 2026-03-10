using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

/// <summary>
/// Throws when response content has unexpected media type
/// </summary>
public class UnexpectedResponseContentTypeException : Exception
{
    /// <summary>
    /// Gets the actual media type of the response content that caused the exception.
    /// </summary>
    public string ActualMediaType { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="UnexpectedResponseContentTypeException"/>
    /// </summary>
    public UnexpectedResponseContentTypeException(string actualMediaType)
        : base($"The response has unexpected media type: '{actualMediaType}'")
    {
        ActualMediaType = actualMediaType;
    }
}