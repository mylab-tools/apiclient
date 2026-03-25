using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Tools;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

namespace MyLab.ApiClient.Usage;

/// <summary>
/// Represents a dynamic proxy for API client contracts, enabling the interception and processing
/// of API requests based on the specified contract and configuration.
/// </summary>
/// <remarks>
/// This class is designed to dynamically create and manage API client proxies for specified contracts.
/// It utilizes reflection and request processing logic to handle API calls, ensuring proper serialization,
/// deserialization, and request execution.
/// </remarks>
public class ApiClientProxy : DispatchProxy
{
    static readonly MethodInfo ProcRequestAsyncMethod;

    static ApiClientProxy()
    {
        ProcRequestAsyncMethod = typeof(ApiClientProxy)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(m => m.Name == nameof(ProcRequestAsync));
    }

    internal ReflectionRequestProcessingLogic? ReflectionRequestSender { get; private set; }

    /// <summary>
    /// Creates a proxy instance for the specified API contract type.
    /// </summary>
    /// <typeparam name="TContract">
    /// The type of the API contract for which the proxy is being created. 
    /// This type must be an interface that defines the API contract.
    /// </typeparam>
    /// <param name="requestProcessor">
    /// The request processor responsible for handling HTTP requests and responses.
    /// </param>
    /// <param name="options">
    /// The options used to configure the API client, including serialization settings and optional HTTP message dumper.
    /// </param>
    /// <returns>
    /// A proxy instance that implements the specified API contract type.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the proxy instance cannot be created.
    /// </exception>
    public static TContract CreateForContract<TContract>
    (
        IRequestProcessor requestProcessor, 
        ApiClientOptions options
    )
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

        object? proxy = Create<TContract, ApiClientProxy>();

        if (proxy == null)
            throw new InvalidOperationException("Can't create proxy");

        var proxyInner = (ApiClientProxy)proxy;
        proxyInner.Initialize(serviceModel, requestProcessor, callDetailsFactory);
            
        return (TContract)proxy;
    }

    /// <inheritdoc />
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) throw new ArgumentNullException(nameof(targetMethod));

        if (ReflectionRequestSender == null) 
            throw new InvalidOperationException("Proxy was not initialized");
            
        if (targetMethod.ReturnType == typeof(Task))
        {
            return ProcRequestForVoidAsync(targetMethod, args);
        }

        if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var retType = targetMethod.ReturnType.GetGenericArguments().First();
                
            return ProcRequestAsyncMethod
                .MakeGenericMethod(retType)
                .Invoke(this, [targetMethod, args]);
        }

        throw new InvalidApiContractException($"Return type '{targetMethod.ReturnType.Name}' is not supported.");
    }

    void Initialize(ServiceModel serviceModel, IRequestProcessor requestProcessor, CallDetailsFactory callDetailsFactory)
    {
        if (serviceModel == null) throw new ArgumentNullException(nameof(serviceModel));
        if (requestProcessor == null) throw new ArgumentNullException(nameof(requestProcessor));
        if (callDetailsFactory == null) throw new ArgumentNullException(nameof(callDetailsFactory));
            
        ReflectionRequestSender = new ReflectionRequestProcessingLogic(serviceModel, requestProcessor, callDetailsFactory);
    }

    async Task ProcRequestForVoidAsync(MethodInfo targetMethod, object?[]? args)
    {
        var callDetails = await ReflectionRequestSender!.SendRequestAsync(targetMethod, args);
        await callDetails.ThrowIfNotOKAsync();
    }

    async Task<T?> ProcRequestAsync<T>(MethodInfo targetMethod, object?[]? args)
    {
        var callDetails = await ReflectionRequestSender!.SendRequestAsync(targetMethod, args);
        await callDetails.ThrowIfNotOKAsync(); 
            
        return await callDetails.ReadContentAsync<T>();
    }
}