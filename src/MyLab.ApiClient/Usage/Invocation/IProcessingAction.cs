using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Defines an action to be performed during the processing of an API call result.
/// </summary>
public interface IProcessingAction
{
    /// <summary>
    /// Gets the predicate that determines whether the processing action should be executed 
    /// based on the provided <see cref="CallDetails"/>.
    /// </summary>
    /// <value>
    /// A delegate of type <see cref="CallDetailsPredicate"/> that evaluates the 
    /// <see cref="CallDetails"/> and returns <c>true</c> if the action should be performed; 
    /// otherwise, <c>false</c>.
    /// </value>
    CallDetailsPredicate Predicate { get; }
    /// <summary>
    /// Executes the processing action asynchronously using the provided call details and cancellation token.
    /// </summary>
    /// <param name="callDetails">
    /// The details of the API call to be processed. Must not be <c>null</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="callDetails"/> is <c>null</c>.
    /// </exception>
    Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken);
}