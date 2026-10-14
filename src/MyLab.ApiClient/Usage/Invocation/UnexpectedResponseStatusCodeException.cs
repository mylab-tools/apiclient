using System;
using System.Net;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Represents an exception that is thrown when an API response status code does not match the expected or handled status codes.
/// </summary>
/// <remarks>
/// This exception is typically used in scenarios where API response validation fails, 
/// and the status code is not among the predefined or handled status codes.
/// </remarks>
public class UnexpectedResponseStatusCodeException : Exception
{
    /// <summary>
    /// Gets the HTTP status code that caused the exception.
    /// </summary>
    /// <value>
    /// The unexpected HTTP status code returned by the API response.
    /// </value>
    /// <remarks>
    /// This property provides the status code that was not expected or handled, 
    /// leading to the exception being thrown. It can be used for debugging or 
    /// logging purposes to identify the issue with the API response.
    /// </remarks>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="UnexpectedResponseStatusCodeException"/>
    /// </summary>
    public UnexpectedResponseStatusCodeException(HttpStatusCode statusCode)
        : base("Unexpected status code")
    {
        StatusCode = statusCode;
    }
}