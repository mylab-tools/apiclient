using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.Problems;
using MyLab.ExpressionTools;

namespace MyLab.ApiClient.Usage.Invocation
{
    /// <summary>
    /// Provides functionality to invoke methods on an API client contract.
    /// </summary>
    /// <typeparam name="TContract">
    /// The type of the API client contract that defines the interface for interacting with the API.
    /// </typeparam>
    public class ApiClientInvoker<TContract>
    {
        readonly ReflectionRequestProcessingLogic _processingLogic;

        ApiClientInvoker(ReflectionRequestProcessingLogic processingLogic)
        {
            _processingLogic = processingLogic ?? throw new ArgumentNullException(nameof(processingLogic));
        }

        /// <summary>
        /// Creates an instance of <see cref="ApiClientInvoker{TContract}"/> for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">
        /// The contract type that defines the API client interface.
        /// </typeparam>
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
            var sender = new ReflectionRequestProcessingLogic(serviceModel, requestProcessor, callDetailsFactory);
            
            return new ApiClientInvoker<TContract>(sender);
        }
        
        /// <summary>
        /// Invokes a method on the API client contract asynchronously.
        /// </summary>
        /// <param name="invocation">
        /// An expression representing the method invocation on the API client contract.
        /// The expression must be a lambda expression that specifies the method to call and its arguments.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an 
        /// <see cref="ApiClientInvocationResult"/> object, which provides details about the API call and its processing context.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="invocation"/> parameter is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="invocation"/> parameter is not a valid lambda expression representing a method call.
        /// </exception>
        public async Task<ApiClientInvocationResult?> InvokeAsync(Expression<Action<TContract>> invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            
            if(!(invocation.Body is MethodCallExpression mce))
            {
                throw new ArgumentException("Must be lambda expression");
            }

            var args = mce.Arguments
                .Select(a => a.GetValue<object>())
                .ToArray();

            var callDetails = await _processingLogic.SendRequestAsync(mce.Method, args);

            return new ApiClientInvocationResult(callDetails, new CallResultProcessingContext());
        }
    }

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
            return ProcessingContext.ProcessAsync(CallDetails, cancellationToken);
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
            var newInvocationResult = new ApiClientInvocationResult(CallDetails, newContext);
            return new ProcessingActionBuilder(newInvocationResult, callDetailsPredicate);
        }
    }

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
    )
    {
        /// <summary>
        /// Processes the result of an API call based on the provided call details and cancellation token.
        /// </summary>
        /// <param name="callDetails">
        /// The details of the API call, including status code, request, and response information.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="callDetails"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="UnexpectedResponseStatusCodeException">
        /// Thrown when the status code of the API response does not match any of the handled status predicates
        /// and <see cref="ThrowIfAnotherStatusCode"/> is set to <c>true</c>.
        /// </exception>
        /// <returns>
        /// A task that represents the asynchronous processing operation.
        /// </returns>
        public async Task ProcessAsync(CallDetails callDetails, CancellationToken cancellationToken)
        {
            if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));
            
            if (ThrowIfAnotherStatusCode)
            {
                if (HandledStatusPredicates == null ||
                    HandledStatusPredicates.All(p => !p(callDetails.StatusCode)))
                {
                    throw new UnexpectedResponseStatusCodeException(callDetails.StatusCode);
                }
            }

            if (ProcessingActions != null)
            {
                foreach (var processingAction in ProcessingActions.Where(a => a.Predicate(callDetails)))
                {
                    await processingAction.PerformAsync(callDetails, cancellationToken);
                }
            }
        }
    };

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

    class ProcessResultAction<TResult> : IProcessingAction
    {
        readonly Action<TResult?, HttpStatusCode> _action;
        public CallDetailsPredicate Predicate { get;}
        
        public ProcessResultAction(Action<TResult?, HttpStatusCode> action, CallDetailsPredicate predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }
        
        public async Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
        {
            if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));

           var result = await callDetails.ReadContentAsync<TResult>();
           _action(result, callDetails.StatusCode);
        }
    }
    
    public class CallError
    {
        public string Message { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }

    /// <summary>
    /// Represents a predicate that evaluates a condition on an HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the condition is met for the specified <paramref name="statusCode"/>; otherwise, <c>false</c>.
    /// </returns>
    public delegate bool StatusCodePredicate(HttpStatusCode statusCode);
    
    /// <summary>
    /// Represents a predicate that evaluates a <see cref="CallDetails"/> instance.
    /// </summary>
    /// <param name="callDetails">The details of the service call to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the predicate condition is satisfied for the provided <paramref name="callDetails"/>; otherwise, <c>false</c>.
    /// </returns>
    public delegate bool CallDetailsPredicate(CallDetails callDetails);
    
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
            :   base("Unexpected status code")
        {
            StatusCode = statusCode;
        }
    }
}
