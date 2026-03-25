using System.Collections.ObjectModel;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Represents the context for processing the result of an API call.
/// </summary>
/// <param name="HandledStatusPredicates">
/// A collection of predicates that define the status codes handled by the processing logic.
/// </param>
/// <param name="ProcessingActions">
/// A collection of actions to be executed during the processing of the API call result.
/// </param>
/// <param name="ThrowIfAnotherStatusCode">
/// Indicates whether an exception should be thrown if the status code of the API response
/// does not match any of the handled status predicates.
/// </param>
public record CallResultProcessingContext(
    ReadOnlyCollection<StatusCodePredicate>? HandledStatusPredicates = null,
    ReadOnlyCollection<IProcessingAction>? ProcessingActions = null,
    bool ThrowIfAnotherStatusCode = false
);