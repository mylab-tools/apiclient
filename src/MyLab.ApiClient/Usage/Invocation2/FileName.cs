using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Problems;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Tools;
using MyLab.ExpressionTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage.Invocation2;

/// <summary>
/// Provides functionality to invoke API client methods defined in a contract interface.
/// </summary>
/// <typeparam name="TContract">
/// The type of the contract interface that defines the API client methods.
/// </typeparam>
/// <remarks>
/// This class is responsible for processing API requests and responses, 
/// creating invocation expressions, and managing the configuration and logic 
/// required to interact with the API.
/// </remarks>
public class ApiClientInvoker<TContract>
{
    readonly ReflectionRequestProcessingLogic _processingLogic;

    ApiClientInvoker(ReflectionRequestProcessingLogic processingLogic)
    {
        _processingLogic = processingLogic ?? throw new ArgumentNullException(nameof(processingLogic));
    }

    /// <summary>
    /// Creates an instance of <see cref="ApiClientInvoker{TContract}"/> from the specified contract type.
    /// </summary>
    /// <typeparam name="TContract">
    /// The type of the contract interface that defines the API client methods.
    /// </typeparam>
    /// <param name="requestProcessor">
    /// An implementation of <see cref="IRequestProcessor"/> used to process HTTP requests.
    /// </param>
    /// <param name="options">
    /// The <see cref="ApiClientOptions"/> containing configuration settings for the API client.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="ApiClientInvoker{TContract}"/> configured for the specified contract type.
    /// </returns>
    /// <remarks>
    /// This method initializes the API client invoker by creating the necessary request processing logic,
    /// including request factoring settings, service model, and response deserialization tools.
    /// </remarks>
    public static ApiClientInvoker<TContract> FromContract(IRequestProcessor requestProcessor, ApiClientOptions options)
    {
        var requestFactoringSettings = RequestFactoringSettings.CreateFromOptions(options);
        var serviceModel = ServiceModel.FromContract(typeof(TContract), requestFactoringSettings);
        var dumper = options.Dumper ?? new HttpMessageDumper();

        var contentDeserializerProvider = SupportedContentDeserializers.Create
        (
            new JsonDeserializationTools(options.JsonSerializer),
            new XmlDeserializationTools()
        );

        var callDetailsFactory = new CallDetailsFactory(contentDeserializerProvider, dumper);
        var processingLogic = new ReflectionRequestProcessingLogic(serviceModel, requestProcessor, callDetailsFactory);

        return new ApiClientInvoker<TContract>(processingLogic);
    }
    
    /// <summary>
    /// Creates an API invocation expression based on the provided contract method expression.
    /// </summary>
    /// <param name="expression">
    /// An expression representing a method of the contract interface <typeparamref name="TContract"/> 
    /// that defines the API call to be invoked.
    /// </param>
    /// <returns>
    /// An object implementing <see cref="IApiInvocationExpression{TContract}"/> that allows further configuration 
    /// and execution of the API invocation.
    /// </returns>
    /// <typeparam name="TContract">
    /// The type of the contract interface that defines the API methods.
    /// </typeparam>
    public IApiInvocationExpression<TContract> ForExpression(Expression<Func<TContract, Task>> expression)
    {
        return new ApiInvocationExpression<TContract>(expression, _processingLogic, new ApiInvocationBuildingState());
    }
}

/// <summary>
/// Represents an API invocation expression that allows configuration and execution of API calls
/// based on a contract interface.
/// </summary>
/// <typeparam name="TContract">
/// The type of the contract interface that defines the API methods to be invoked.
/// </typeparam>
public interface IApiInvocationExpression<TContract>
{
    /// <summary>
    /// Configures processing logic for specific HTTP status codes.
    /// </summary>
    /// <param name="code">The primary HTTP status code to handle.</param>
    /// <param name="anotherCodes">Additional HTTP status codes to handle.</param>
    IApiExpressionConditionalStarter<TContract> When(HttpStatusCode code, params HttpStatusCode[] anotherCodes);

    /// <summary>
    /// Configures processing logic specifically for HTTP 1xx (Informational) status codes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    IApiExpressionConditionalStarter<TContract> When1xx();

    /// <summary>
    /// Configures processing logic specifically for HTTP 2xx (Successful) status codes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    IApiExpressionConditionalStarter<TContract> When2xx();

    /// <summary>
    /// Configures processing logic specifically for HTTP 3xx (Redirection) status codes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    IApiExpressionConditionalStarter<TContract> When3xx();

    /// <summary>
    /// Configures processing logic specifically for HTTP 4xx (Client Error) status codes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    IApiExpressionConditionalStarter<TContract> When4xx();

    /// <summary>
    /// Configures processing logic specifically for HTTP 5xx (Server Error) status codes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    IApiExpressionConditionalStarter<TContract> When5xx();

    /// <summary>
    /// Ensures that an exception is thrown if the HTTP response status code does not match any of the configured status codes.
    /// </summary>
    /// <returns>
    /// The current API invocation expression, allowing for further configuration or execution of the API call.
    /// </returns>
    IApiInvocationExpression<TContract> ThrowIfAnotherStatusCode();

    /// <summary>
    /// Executes the API call asynchronously based on the configured invocation expression.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains detailed information
    /// about the service call, including the HTTP request and response details.
    /// </returns>
    Task<CallDetails> InvokeAsync(CancellationToken cancellationToken = default);
}

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

interface IApiInvocationExpressionFactory<TContract>
{
    IApiInvocationExpression<TContract> CreateExpression(ApiInvocationBuildingState buildingState);
}

class ApiInvocationExpression<TContract>
    (
        Expression<Func<TContract, Task>> expression,
        ReflectionRequestProcessingLogic processingLogic,
        ApiInvocationBuildingState buildingState
    ) : 
        IApiInvocationExpression<TContract>, 
        IApiInvocationExpressionFactory<TContract>
{
    public IApiExpressionConditionalStarter<TContract> When(HttpStatusCode code, params HttpStatusCode[] anotherCodes)
    {
        var predicateList = new List<StatusCodePredicate>
        {
            c => c == code
        };

        if (buildingState.HandledStatusPredicates != null)
        {
            predicateList.AddRange(buildingState.HandledStatusPredicates);
        }

        predicateList.AddRange
        (
            anotherCodes.Select(c => (StatusCodePredicate)(ac => ac == c))
        );

        var newState = buildingState with
        {
            HandledStatusPredicates = new ReadOnlyCollection<StatusCodePredicate>(predicateList)
        };

        var detailsPredicate = (CallDetailsPredicate)(d => predicateList.Any(p => p(d.StatusCode)));

        return new ApiExpressionConditionalStarter<TContract>(this, newState, detailsPredicate);
    }

    public IApiExpressionConditionalStarter<TContract> When1xx() => WhenXxx(100);

    public IApiExpressionConditionalStarter<TContract> When2xx() => WhenXxx(200);

    public IApiExpressionConditionalStarter<TContract> When3xx() => WhenXxx(300);

    public IApiExpressionConditionalStarter<TContract> When4xx() => WhenXxx(400);

    public IApiExpressionConditionalStarter<TContract> When5xx() => WhenXxx(500);

    public IApiInvocationExpression<TContract> ThrowIfAnotherStatusCode()
    {
        return CreateExpression(buildingState with { ThrowIfAnotherStatusCode = true });
    }

    public Task<CallDetails> InvokeAsync(CancellationToken cancellationToken)
    {
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        if (!(expression.Body is MethodCallExpression mce))
            throw new ArgumentException("Must be lambda expression");

        var args = ExtractArgs(mce);

        return processingLogic.SendRequestAsync(mce.Method, args);
    }

    public IApiInvocationExpression<TContract> CreateExpression(ApiInvocationBuildingState newBuildingState)
    {
        return new ApiInvocationExpression<TContract>(expression, processingLogic, newBuildingState);
    }

    ApiExpressionConditionalStarter<TContract> WhenXxx(int startCode)
    {
        var predicateList = new List<StatusCodePredicate>
        {
            c => (int)c >= startCode && (int)c < startCode + 100
        };

        if (buildingState.HandledStatusPredicates != null)
        {
            predicateList.AddRange(buildingState.HandledStatusPredicates);
        }

        var newState = buildingState with
        {
            HandledStatusPredicates = new ReadOnlyCollection<StatusCodePredicate>(predicateList)
        };

        var callDetailsPredicate = (CallDetailsPredicate)(d => (int)d.StatusCode >= startCode && (int)d.StatusCode < startCode + 100);

        return new ApiExpressionConditionalStarter<TContract>(this, newState, callDetailsPredicate);
    }

    static object[] ExtractArgs(MethodCallExpression mce)
    {
        var args = mce.Arguments
            .Select(a => a.GetValue<object>())
            .ToArray();
        return args;
    }
}

class ApiExpressionConditionalStarter<TContract> 
(
    IApiInvocationExpressionFactory<TContract> expressionFactory,
    ApiInvocationBuildingState buildingState,
    CallDetailsPredicate callDetailsPredicate
)
    : IApiExpressionConditionalStarter<TContract>
{
    public IApiInvocationExpression<TContract> ProcessResult<TResult>(Action<TResult?, HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessResultAction<TResult>(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessStatusCode(Action<HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessStatusCodeAction(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessProblem(Action<ProblemDetails?, HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessResultAction<ProblemDetails>(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessValidationProblem(Action<ValidationProblemDetails?, HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessResultAction<ValidationProblemDetails>(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessResultAsync<TResult>(Func<TResult?, HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessResultAction<TResult>(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    public IApiInvocationExpression<TContract> ProcessStatusCodeAsync(Func<HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessStatusCodeAction(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    public IApiInvocationExpression<TContract> ProcessProblemAsync(Func<ProblemDetails?, HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessResultAction<ProblemDetails?>(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    public IApiInvocationExpression<TContract> ProcessValidationProblemAsync(Func<ValidationProblemDetails?, HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessResultAction<ValidationProblemDetails?>(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    IApiInvocationExpression<TContract> AddProcAction(IProcessingAction act)
    {
        var acts = buildingState.ProcessingActions != null
            ? [.. buildingState.ProcessingActions]
            : new List<IProcessingAction>();

        acts.Add(act);

        var newBuildState = buildingState with
        {
            ProcessingActions = new ReadOnlyCollection<IProcessingAction>(acts)
        };
        return expressionFactory.CreateExpression(newBuildState);
    }
}

record ApiInvocationBuildingState(
    ReadOnlyCollection<StatusCodePredicate>? HandledStatusPredicates = null,
    ReadOnlyCollection<IProcessingAction>? ProcessingActions = null,
    bool ThrowIfAnotherStatusCode = false
);

/// <summary>
/// Represents a predicate that evaluates a condition on an HTTP status code.
/// </summary>
/// <param name="statusCode">The HTTP status code to evaluate.</param>
/// <returns>
/// <c>true</c> if the condition is met for the specified <paramref name="statusCode"/>; otherwise, <c>false</c>.
/// </returns>
public delegate bool StatusCodePredicate(HttpStatusCode statusCode);

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

class ProcessResultAction<TResult>
    (
        Action<TResult?, HttpStatusCode> action,
        CallDetailsPredicate predicate
        )
    : IProcessingAction
{
    public CallDetailsPredicate Predicate => predicate;

    public async Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));

        var result = await callDetails.ReadContentAsync<TResult>();
        action(result, callDetails.StatusCode);
    }
}

class ProcessStatusCodeAction
(
    Action<HttpStatusCode> action,
    CallDetailsPredicate predicate
)
    : IProcessingAction
{
    public CallDetailsPredicate Predicate => predicate;

    public Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));
        action(callDetails.StatusCode);

        return Task.CompletedTask;
    }
}

class AsyncProcessResultAction<TResult>
(
    Func<TResult?, HttpStatusCode, Task> func,
    CallDetailsPredicate predicate
)
    : IProcessingAction
{
    public CallDetailsPredicate Predicate => predicate;

    public async Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));

        var result = await callDetails.ReadContentAsync<TResult>();
        await func(result, callDetails.StatusCode);
    }
}

class AsyncProcessStatusCodeAction
(
    Func<HttpStatusCode, Task> func,
    CallDetailsPredicate predicate
)
    : IProcessingAction
{
    public CallDetailsPredicate Predicate => predicate;

    public async Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));
        await func(callDetails.StatusCode);
    }
}

/// <summary>
/// Represents a predicate that evaluates a <see cref="CallDetails"/> instance.
/// </summary>
/// <param name="callDetails">The details of the service call to evaluate.</param>
/// <returns>
/// <c>true</c> if the predicate condition is satisfied for the provided <paramref name="callDetails"/>; otherwise, <c>false</c>.
/// </returns>
public delegate bool CallDetailsPredicate(CallDetails callDetails);