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
        public async Task<CallDetails> SendRequestAsync(MethodInfo targetMethod, object?[]? args)
        {
            if (!serviceModel.Endpoints.TryGetValue(targetMethod.MetadataToken, out var endpointModel))
                throw new InvalidOperationException($"Endpoint description not found for method '{targetMethod.Name}'.");
            
            var requestFactory = new RequestFactory(serviceModel, endpointModel);
            var request = requestFactory.Create(args ?? []);

            var ct = args != null
                ? args.OfType<CancellationToken>().FirstOrDefault(CancellationToken.None)
                : CancellationToken.None;
            
            var response = await requestProcessor.ProcessRequestAsync(request, ct);
            return await callDetailsFactory.CreateAsync(request, response);
        }
    }
}
