using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Represents a predicate that evaluates a <see cref="CallDetails"/> instance.
/// </summary>
/// <param name="callDetails">The details of the service call to evaluate.</param>
/// <returns>
/// <c>true</c> if the predicate condition is satisfied for the provided <paramref name="callDetails"/>; otherwise, <c>false</c>.
/// </returns>
public delegate bool CallDetailsPredicate(CallDetails callDetails);