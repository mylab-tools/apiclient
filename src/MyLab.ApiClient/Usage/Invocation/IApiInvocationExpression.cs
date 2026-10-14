using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

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