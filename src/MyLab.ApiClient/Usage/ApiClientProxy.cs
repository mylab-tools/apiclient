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

        public ReflectionRequestProcessingLogic? ReflectionRequestSender { get; private set; }

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
}
