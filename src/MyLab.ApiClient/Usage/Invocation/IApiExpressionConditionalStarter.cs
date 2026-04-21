using System;
using System.Net;
using System.Threading.Tasks;
using MyLab.ApiClient.Problems;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Represents a conditional starter interface for configuring API invocation expressions 
/// based on specific HTTP status codes or conditions.
/// </summary>
/// <typeparam name="TContract">
/// The type of the API client contract associated with the invocation.
/// </typeparam>
public interface IApiExpressionConditionalStarter<TContract>
{
    /// <summary>
    /// Configures the processing of the result for the current API client invocation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be processed.</typeparam>
    /// <param name="action">
    /// The action to be executed for processing the result. 
    /// It receives the result of type <typeparamref name="TResult"/> (which can be <c>null</c>) 
    /// and the HTTP status code of the response.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="action"/> parameter is <c>null</c>.
    /// </exception>
    IApiInvocationExpression<TContract> ProcessResult<TResult>(Action<TResult?, HttpStatusCode> action);

    /// <summary>
    /// Configures the API invocation expression to process the HTTP status code of the response
    /// using the specified action.
    /// </summary>
    /// <param name="action">
    /// An action to be executed with the HTTP status code of the response.
    /// </param>
    /// <returns>
    /// The API invocation expression for further configuration or execution.
    /// </returns>
    IApiInvocationExpression<TContract> ProcessStatusCode(Action<HttpStatusCode> action);

    /// <summary>
    /// Configures a processing action to handle API invocation results as problems.
    /// </summary>
    /// <param name="action">
    /// A delegate that processes the <see cref="ProblemDetails"/> object and the associated HTTP status code.
    /// </param>
    /// <remarks>
    /// This method is used to define custom logic for handling API invocation results that are represented
    /// as problems, typically conforming to the Problem Details standard (RFC 7807).
    /// </remarks>
    IApiInvocationExpression<TContract> ProcessProblem(Action<ProblemDetails?, HttpStatusCode> action);

    /// <summary>
    /// Configures a processing action to handle validation problems encountered during an API client invocation.
    /// </summary>
    /// <param name="action">
    /// The action to execute when a validation problem occurs. The action receives the 
    /// <see cref="ValidationProblemDetails"/> instance and the associated HTTP status code as parameters.
    /// </param>
    /// <remarks>
    /// Use this method to define custom logic for handling validation problems, such as logging errors
    /// or transforming validation details into a specific format.
    /// </remarks>
    IApiInvocationExpression<TContract> ProcessValidationProblem(Action<ValidationProblemDetails?, HttpStatusCode> action);

    /// <summary>
    /// Configures the asynchronous processing of the result for the current API client invocation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be processed.</typeparam>
    /// <param name="func">
    /// The asynchronous function to be executed for processing the result. 
    /// It receives the result of type <typeparamref name="TResult"/> (which can be <c>null</c>) 
    /// and the HTTP status code of the response.
    /// </param>
    /// <returns>
    /// The current API invocation expression, allowing further configuration or execution.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="func"/> parameter is <c>null</c>.
    /// </exception>
    IApiInvocationExpression<TContract> ProcessResultAsync<TResult>(Func<TResult?, HttpStatusCode, Task> func);

    /// <summary>
    /// Configures the API invocation expression to process the HTTP status code of the response
    /// asynchronously using the specified function.
    /// </summary>
    /// <param name="func">
    /// A function to be executed asynchronously with the HTTP status code of the response.
    /// </param>
    /// <returns>
    /// The API invocation expression for further configuration or execution.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="func"/> parameter is <c>null</c>.
    /// </exception>
    IApiInvocationExpression<TContract> ProcessStatusCodeAsync(Func<HttpStatusCode, Task> func);

    /// <summary>
    /// Configures an asynchronous processing action to handle API invocation results as problems.
    /// </summary>
    /// <param name="func">
    /// A delegate that processes the <see cref="ProblemDetails"/> object and the associated HTTP status code asynchronously.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IApiInvocationExpression{TContract}"/> to allow further configuration or execution of the API invocation.
    /// </returns>
    /// <remarks>
    /// This method is used to define custom asynchronous logic for handling API invocation results that are represented
    /// as problems, typically conforming to the Problem Details standard (RFC 7807).
    /// </remarks>
    IApiInvocationExpression<TContract> ProcessProblemAsync(Func<ProblemDetails?, HttpStatusCode, Task> func);

    /// <summary>
    /// Configures an asynchronous processing action to handle validation problems encountered during an API client invocation.
    /// </summary>
    /// <param name="func">
    /// The asynchronous function to execute when a validation problem occurs. The function receives the 
    /// <see cref="ValidationProblemDetails"/> instance and the associated HTTP status code as parameters.
    /// </param>
    /// <returns>
    /// An updated API invocation expression that allows further configuration or execution of the API call.
    /// </returns>
    /// <remarks>
    /// Use this method to define custom asynchronous logic for handling validation problems, such as logging errors,
    /// transforming validation details into a specific format, or triggering additional asynchronous operations.
    /// </remarks>
    IApiInvocationExpression<TContract> ProcessValidationProblemAsync(Func<ValidationProblemDetails?, HttpStatusCode, Task> func);
}