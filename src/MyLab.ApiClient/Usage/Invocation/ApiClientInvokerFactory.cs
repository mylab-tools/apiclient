using System;
using System.Reflection;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Provides a factory for creating API client invokers based on contract types.
/// </summary>
/// <remarks>
/// This class is responsible for generating instances of <see cref="IApiClientInvoker{TContract}"/> 
/// using the provided request processor and API client options. It supports both generic and 
/// non-generic contract types.
/// </remarks>
public static class ApiClientInvokerFactory
{
    internal static readonly MethodInfo FromContractMethod = typeof(ApiClientInvokerFactory).GetMethod(
        nameof(FromContract),
        BindingFlags.Instance | BindingFlags.Public,
        Array.Empty<Type>()
    )!;

    /// <summary>
    /// Creates an API client invoker for the specified contract type.
    /// </summary>
    /// <typeparam name="TContract">
    /// The type of the contract for which the API client invoker is created. 
    /// This type defines the API contract that the invoker will adhere to.
    /// </typeparam>
    /// <returns>
    /// An instance of <see cref="IApiClientInvoker{TContract}"/> that can be used to invoke API methods
    /// defined in the specified contract.
    /// </returns>
    /// <remarks>
    /// This method initializes the necessary components, such as request factoring settings,
    /// service model, content deserializers, and processing logic, to create a fully functional
    /// API client invoker.
    /// </remarks>
    public static IApiClientInvoker<TContract> FromContract<TContract>(IRequestProcessor requestProcessor, ApiClientOptions options)
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
}