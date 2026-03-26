using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Problems;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Tools;
using MyLab.ApiClient.Usage.Invocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage.Invocation2;

public class ApiClientInvoker<TContract>
{
    readonly ReflectionRequestProcessingLogic _processingLogic;

    ApiClientInvoker(ReflectionRequestProcessingLogic processingLogic)
    {
        _processingLogic = processingLogic ?? throw new ArgumentNullException(nameof(processingLogic));
    }

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

    public IApiInvocationExpression<TContract> ForExpression(Expression<Func<TContract, Task>> expression)
    {
        return new ApiInvocationExpression<TContract>(expression, _processingLogic);
    }
}

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

    IApiInvocationExpression<TContract> ThrowIfAnotherStatusCode();

    Task<CallDetails> InvokeAsync<TResult>(CancellationToken cancellationToken);
}

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
}

class ApiInvocationExpression<TContract>
    (
        Expression<Func<TContract, Task>> expression,
        ReflectionRequestProcessingLogic processingLogic
    )
    : IApiInvocationExpression<TContract>
{

}