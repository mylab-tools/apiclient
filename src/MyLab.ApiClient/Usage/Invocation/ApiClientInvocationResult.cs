using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Represents the result of an API client invocation, including details about the API call
/// and the context for processing the call result.
/// </summary>
/// <remarks>
/// This record encapsulates the details of an API call, such as the call's metadata and
/// the processing context, which defines how the result of the call should be handled.
/// </remarks>
public record ApiClientInvocationResult(CallDetails CallDetails, CallResultProcessingContext ProcessingContext)
{
    /// <summary>
    /// Gets or sets the strategy used for processing the result of an API client invocation.
    /// </summary>
    public IFluentProcessingStrategy ProcessingStrategy { get; set; } = new DefaultFluentProcessingStrategy();

    /// <summary>
    /// Configures processing logic for specific HTTP status codes.
    /// </summary>
    /// <param name="code">The primary HTTP status code to handle.</param>
    /// <param name="anotherCodes">Additional HTTP status codes to handle.</param>
    /// <returns>A <see cref="ProcessingActionBuilder"/> instance to define further processing actions.</returns>
    public ProcessingActionBuilder When(HttpStatusCode code, params HttpStatusCode[] anotherCodes)
    {
        var predicateList = new List<StatusCodePredicate>
        {
            c => c == code
        };

        if (ProcessingContext.HandledStatusPredicates != null)
        {
            predicateList.AddRange(ProcessingContext.HandledStatusPredicates);
        }

        predicateList.AddRange
        (
            anotherCodes.Select(c => (StatusCodePredicate)(ac => ac == c))
        );

        var newContext = ProcessingContext with
        {
            HandledStatusPredicates = new ReadOnlyCollection<StatusCodePredicate>(predicateList)
        };

        var detailsPredicate = (CallDetailsPredicate)(d => predicateList.Any(p => p(d.StatusCode)));
            
        return ToProcessingAction(newContext, detailsPredicate);
    }

    /// <summary>
    /// Configures processing logic specifically for HTTP 1xx (Informational) status codes.
    /// </summary>
    /// <returns>
    /// A <see cref="ProcessingActionBuilder"/> instance to define further processing actions
    /// for HTTP 1xx status codes.
    /// </returns>
    // ReSharper disable once InconsistentNaming
    public ProcessingActionBuilder When1xx() => WhenXxx(100);
    /// <summary>
    /// Configures processing logic specifically for HTTP 2xx (Successful) status codes.
    /// </summary>
    /// <returns>
    /// A <see cref="ProcessingActionBuilder"/> instance to define further processing actions
    /// for HTTP 2xx status codes.
    /// </returns>
    // ReSharper disable once InconsistentNaming
    public ProcessingActionBuilder When2xx() => WhenXxx(200);
    /// <summary>
    /// Configures processing logic specifically for HTTP 3xx (Redirection) status codes.
    /// </summary>
    /// <returns>
    /// A <see cref="ProcessingActionBuilder"/> instance to define further processing actions
    /// for HTTP 3xx status codes.
    /// </returns>
    // ReSharper disable once InconsistentNaming
    public ProcessingActionBuilder When3xx() => WhenXxx(300);
    /// <summary>
    /// Configures processing logic specifically for HTTP 4xx (Client Error) status codes.
    /// </summary>
    /// <returns>
    /// A <see cref="ProcessingActionBuilder"/> instance to define further processing actions
    /// for HTTP 4xx status codes.
    /// </returns>
    // ReSharper disable once InconsistentNaming
    public ProcessingActionBuilder When4xx() => WhenXxx(400);
    /// <summary>
    /// Configures processing logic specifically for HTTP 5xx (Server Error) status codes.
    /// </summary>
    /// <returns>
    /// A <see cref="ProcessingActionBuilder"/> instance to define further processing actions
    /// for HTTP 5xx status codes.
    /// </returns>
    // ReSharper disable once InconsistentNaming
    public ProcessingActionBuilder When5xx() => WhenXxx(500);

    /// <summary>
    /// Throws an exception if the HTTP response status code does not match any of the configured status codes.
    /// </summary>
    /// <returns>
    /// A new <see cref="ApiClientInvocationResult"/> instance with the updated processing context
    /// that enforces throwing an exception for unhandled status codes.
    /// </returns>
    public ApiClientInvocationResult ThrowIfAnotherStatusCode()
    {
        var newContext = ProcessingContext with
        {
            ThrowIfAnotherStatusCode = true
        };
        return this with { ProcessingContext = newContext };
    }

    /// <summary>
    /// Executes the processing logic defined in the <see cref="CallResultProcessingContext"/> 
    /// for the current API call details.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the processing operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="CallDetails"/> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided <paramref name="cancellationToken"/>.
    /// </exception>
    public Task ProcessAsync(CancellationToken cancellationToken)
    {
        return ProcessingStrategy.ProcessAsync(CallDetails, ProcessingContext, cancellationToken);
    }

    ProcessingActionBuilder WhenXxx(int startCode)
    {
        var predicateList = new List<StatusCodePredicate>
        {
            c => (int)c >= startCode && (int)c < startCode + 100
        };

        if (ProcessingContext.HandledStatusPredicates != null)
        {
            predicateList.AddRange(ProcessingContext.HandledStatusPredicates);
        }

        var newContext = ProcessingContext with
        {
            HandledStatusPredicates = new ReadOnlyCollection<StatusCodePredicate>(predicateList)
        };
            
        var callDetailsPredicate = (CallDetailsPredicate)(d => (int)d.StatusCode >= startCode && (int)d.StatusCode < startCode + 100);
            
        return ToProcessingAction(newContext,callDetailsPredicate);
    }

    ProcessingActionBuilder ToProcessingAction(CallResultProcessingContext newContext, CallDetailsPredicate callDetailsPredicate)
    {
        var newInvocationResult = this with { ProcessingContext = newContext };
        return new ProcessingActionBuilder(newInvocationResult, callDetailsPredicate);
    }
}