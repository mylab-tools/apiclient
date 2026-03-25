using System.Net;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Represents a predicate that evaluates a condition on an HTTP status code.
/// </summary>
/// <param name="statusCode">The HTTP status code to evaluate.</param>
/// <returns>
/// <c>true</c> if the condition is met for the specified <paramref name="statusCode"/>; otherwise, <c>false</c>.
/// </returns>
public delegate bool StatusCodePredicate(HttpStatusCode statusCode);