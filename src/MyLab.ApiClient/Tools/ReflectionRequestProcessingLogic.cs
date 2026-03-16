using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Usage;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Tools
{
    class ReflectionRequestProcessingLogic
    (
        ServiceModel serviceModel,
        IRequestProcessor requestProcessor, 
        CallDetailsFactory callDetailsFactory
    )
    {
        public ServiceModel ServiceModel { get; } = serviceModel;
        public IRequestProcessor RequestProcessor { get; } = requestProcessor;
        public CallDetailsFactory CallDetailsFactory { get; } = callDetailsFactory;


        public async Task<CallDetails> SendRequestAsync(MethodInfo targetMethod, object?[]? args)
        {
            if (!ServiceModel.Endpoints.TryGetValue(targetMethod.MetadataToken, out var endpointModel))
                throw new InvalidOperationException($"Endpoint description not found for method '{targetMethod.Name}'.");
            
            var requestFactory = new RequestFactory(ServiceModel, endpointModel);
            var request = requestFactory.Create(args ?? []);

            var ct = args != null
                ? args.OfType<CancellationToken>().FirstOrDefault(CancellationToken.None)
                : CancellationToken.None;
            
            var response = await RequestProcessor.ProcessRequestAsync(request, ct);
            return await CallDetailsFactory.CreateAsync(request, response);
        }
    }
}
