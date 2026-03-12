using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Tools;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

namespace MyLab.ApiClient.Usage
{
    class ApiClientProxy : DispatchProxy
    {
        static readonly MethodInfo ProcRequestAsyncMethod;

        static ApiClientProxy()
        {
            ProcRequestAsyncMethod = typeof(ApiClientProxy)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(m => m.Name == nameof(ProcRequestAsync));
        }

        public ServiceModel? ServiceModel { get; private set; }
        public IRequestProcessor? RequestProcessor { get; private set; }
        public CallDetailsFactory? CallDetailsFactory { get; private set; }

        public static TContract CreateFroContract<TContract>
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
            
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null) throw new ArgumentNullException(nameof(targetMethod));

            if (ServiceModel == null) throw new InvalidOperationException("Proxy was not initialized");
            
            if (!ServiceModel.Endpoints.TryGetValue(targetMethod.MetadataToken, out var endpointModel))
                throw new InvalidOperationException($"Endpoint description not found for method '{targetMethod.Name}'.");

            var requestFactory = new RequestFactory(ServiceModel, endpointModel);
            var request = requestFactory.Create(args ?? []);

            var cancellationToken = CancellationTokenExtractor.FromMethodInvocation(targetMethod, args);

            if (targetMethod.ReturnType == typeof(Task))
            {
                return ProcRequestForVoidAsync(request, cancellationToken);
            }

            if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var retType = targetMethod.ReturnType.GetGenericArguments().First();
                
                return ProcRequestAsyncMethod
                    .MakeGenericMethod(retType)
                    .Invoke(this, [request, cancellationToken]);
            }

            throw new InvalidApiContractException($"Return type '{targetMethod.ReturnType.Name}' is not supported.");
        }

        void Initialize(ServiceModel serviceModel, IRequestProcessor requestProcessor, CallDetailsFactory callDetailsFactory)
        {
            ServiceModel = serviceModel ?? throw new ArgumentNullException(nameof(serviceModel));
            RequestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
            CallDetailsFactory = callDetailsFactory ?? throw new ArgumentNullException(nameof(callDetailsFactory));
        }

        async Task ProcRequestForVoidAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (RequestProcessor == null || CallDetailsFactory == null) 
                throw new InvalidOperationException("Proxy was not initialized");

            var response = await RequestProcessor.ProcessRequestAsync(request, ct);

            var callDetails = await CallDetailsFactory.CreateAsync(request, response);
            await callDetails.ThrowIfNotOKAsync();
        }

        async Task<T?> ProcRequestAsync<T>(HttpRequestMessage request, CancellationToken ct)
        {
            if (RequestProcessor == null || CallDetailsFactory == null)
                throw new InvalidOperationException("Proxy was not initialized");
            
            var response = await RequestProcessor.ProcessRequestAsync(request, ct);

            var callDetails = await CallDetailsFactory.CreateAsync(request, response);
            await callDetails.ThrowIfNotOKAsync(); 
            
            return await callDetails.ReadContentAsync<T>();
        }
    }
}
