using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Tools;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage.Invocation;

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