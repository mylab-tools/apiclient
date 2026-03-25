using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Defines a strategy for fluent processing of API client invocation results.
/// </summary>
public interface IFluentProcessingStrategy
{
    /// <summary>
    /// Processes the result of an API client invocation asynchronously.
    /// </summary>
    /// <param name="callDetails">
    /// Contains detailed information about the API call, including request and response data.
    /// </param>
    /// <param name="processingContext">
    /// The context for processing the API call result, which includes status predicates and processing actions.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task ProcessAsync(CallDetails callDetails, CallResultProcessingContext processingContext, CancellationToken cancellationToken);
}