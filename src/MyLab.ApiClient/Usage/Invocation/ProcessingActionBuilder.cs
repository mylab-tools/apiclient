using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using MyLab.ApiClient.Problems;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Provides a builder for configuring processing actions for API client invocation results.
/// </summary>
/// <remarks>
/// This class is used to define custom processing logic for API client invocation results,
/// such as handling specific HTTP status codes or processing errors.
/// </remarks>
public class ProcessingActionBuilder
{
    readonly ApiClientInvocationResult _invocationResult;
    readonly CallDetailsPredicate _callDetailsPredicate;

    /// <summary>
    /// Initializes a new instance of <see cref="ProcessingActionBuilder"/>
    /// </summary>
    public ProcessingActionBuilder(ApiClientInvocationResult invocationResult, CallDetailsPredicate callDetailsPredicate)
    {
        _invocationResult = invocationResult ?? throw new ArgumentNullException(nameof(invocationResult));
        _callDetailsPredicate = callDetailsPredicate;
    }

    /// <summary>
    /// Configures the processing of the result for the current API client invocation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be processed.</typeparam>
    /// <param name="action">
    /// The action to be executed for processing the result. 
    /// It receives the result of type <typeparamref name="TResult"/> (which can be <c>null</c>) 
    /// and the HTTP status code of the response.
    /// </param>
    /// <returns>
    /// An updated <see cref="ApiClientInvocationResult"/> instance with the configured processing action.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="action"/> parameter is <c>null</c>.
    /// </exception>
    public ApiClientInvocationResult ProcessResult<TResult>(Action<TResult?, HttpStatusCode> action)
    {
        var act = new ProcessResultAction<TResult>(action, _callDetailsPredicate);
        return AddProcAction(act);
    }
        
    /// <summary>
    /// Configures a processing action to handle API invocation results as problems.
    /// </summary>
    /// <param name="action">
    /// A delegate that processes the <see cref="ProblemDetails"/> object and the associated HTTP status code.
    /// </param>
    /// <returns>
    /// The <see cref="ApiClientInvocationResult"/> instance to allow further configuration or processing.
    /// </returns>
    /// <remarks>
    /// This method is used to define custom logic for handling API invocation results that are represented
    /// as problems, typically conforming to the Problem Details standard (RFC 7807).
    /// </remarks>
    public ApiClientInvocationResult ProcessProblem(Action<ProblemDetails?, HttpStatusCode> action)
    {
        var act = new ProcessResultAction<ProblemDetails>(action, _callDetailsPredicate);
        return AddProcAction(act);
    }

    /// <summary>
    /// Configures a processing action to handle validation problems encountered during an API client invocation.
    /// </summary>
    /// <param name="action">
    /// The action to execute when a validation problem occurs. The action receives the 
    /// <see cref="ValidationProblemDetails"/> instance and the associated HTTP status code as parameters.
    /// </param>
    /// <returns>
    /// The <see cref="ApiClientInvocationResult"/> instance, allowing further configuration or processing.
    /// </returns>
    /// <remarks>
    /// Use this method to define custom logic for handling validation problems, such as logging errors
    /// or transforming validation details into a specific format.
    /// </remarks>
    public ApiClientInvocationResult ProcessValidationProblem(Action<ValidationProblemDetails?, HttpStatusCode> action)
    {
        var act = new ProcessResultAction<ValidationProblemDetails>(action, _callDetailsPredicate);
        return AddProcAction(act);
    }

    ApiClientInvocationResult AddProcAction(IProcessingAction act)
    {
        var acts = _invocationResult.ProcessingContext.ProcessingActions != null
            ? [.. _invocationResult.ProcessingContext.ProcessingActions]
            : new List<IProcessingAction>();

        acts.Add(act);

        return _invocationResult with
        {
            ProcessingContext = _invocationResult.ProcessingContext with
            {
                ProcessingActions = new ReadOnlyCollection<IProcessingAction>(acts)
            }
        };
    }
}