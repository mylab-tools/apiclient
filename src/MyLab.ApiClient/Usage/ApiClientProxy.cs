using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring;
using MyLab.ApiClient.ResponseProcessing;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

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

        ServiceModel? _serviceModel;
        IRequestProcessor? _requestProcessor;
        CallDetailsFactory? _callDetailsFactory;


        public static TContract CreateFroContract<TContract>
        (
            IRequestProcessor requestProcessor, 
            ApiClientOptions options
        )
        {
            var requestFactoringSettings = RequestFactoringSettings.CreateFromOptions(options);
            var serviceModel = ServiceModel.FromContract(typeof(TContract), requestFactoringSettings);
            var callDetailsFactory = new CallDetailsFactory(options);

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

            if (_serviceModel == null) throw new InvalidOperationException("Proxy was not initialized");
            
            if (!_serviceModel.Endpoints.TryGetValue(targetMethod.MetadataToken, out var endpointModel))
                throw new InvalidOperationException($"Endpoint description not found for method '{targetMethod.Name}'.");

            var requestFactory = new RequestFactory(_serviceModel, endpointModel);
            var request = requestFactory.Create(args ?? []);

            if (targetMethod.ReturnType == typeof(Task))
            {
                return ProcRequestForVoidAsync(request);
            }

            if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var retType = targetMethod.ReturnType.GetGenericArguments().First();
                
                return ProcRequestAsyncMethod
                    .MakeGenericMethod(retType)
                    .Invoke(this, [request]);
            }

            throw new InvalidApiContractException($"Return type '{targetMethod.ReturnType.Name}' is not supported.");
        }

        void Initialize(ServiceModel serviceModel, IRequestProcessor requestProcessor, CallDetailsFactory callDetailsFactory)
        {
            _serviceModel = serviceModel ?? throw new ArgumentNullException(nameof(serviceModel));
            _requestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
            _callDetailsFactory = callDetailsFactory ?? throw new ArgumentNullException(nameof(callDetailsFactory));
        }

        async Task ProcRequestForVoidAsync(HttpRequestMessage request)
        {
            if (_requestProcessor == null || _callDetailsFactory == null) 
                throw new InvalidOperationException("Proxy was not initialized");

            var response = await _requestProcessor.ProcessRequestAsync(request);

            var callDetails = await _callDetailsFactory.CreateAsync(request, response);
            await callDetails.ThrowIfNotOKAsync();
        }

        async Task<T?> ProcRequestAsync<T>(HttpRequestMessage request)
        {
            if (_requestProcessor == null || _callDetailsFactory == null)
                throw new InvalidOperationException("Proxy was not initialized");
            
            var response = await _requestProcessor.ProcessRequestAsync(request);

            var callDetails = await _callDetailsFactory.CreateAsync(request, response);
            await callDetails.ThrowIfNotOKAsync(); 
            
            return await callDetails.ReadContentAsync<T>();
        }
    }
}
