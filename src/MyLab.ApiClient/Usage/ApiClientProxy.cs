using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring;
using MyLab.ApiClient.ResponseProcessing;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage
{
    class ApiClientProxy : DispatchProxy
    {
        readonly ServiceModel _serviceModel;
        readonly IRequestProcessor _requestProcessor;

        public ApiClientProxy(ServiceModel serviceModel, IRequestProcessor requestProcessor)
        {
            _serviceModel = serviceModel ?? throw new ArgumentNullException(nameof(serviceModel));
            _requestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
        }
        
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null) throw new ArgumentNullException(nameof(targetMethod));

            if (!_serviceModel.Endpoints.TryGetValue(targetMethod.MetadataToken, out var endpointModel))
                throw new InvalidOperationException($"Endpoint description not found for method '{targetMethod.Name}'.");

            var requestFactory = new RequestFactory(_serviceModel, endpointModel);
            var request = requestFactory.Create(args ?? []);

            // Process request using the request processor
            var processedRequestTask = _requestProcessor.ProcessRequestAsync(request);
            processedRequestTask.Wait();

            var response = processedRequestTask.Result;

            CheckForExpectedCode(endpointModel, response);

            // Handle the return type of the method
            if (targetMethod.ReturnType == typeof(Task)) return Task.CompletedTask;

            if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                //var resultType = targetMethod.ReturnType.GetGenericArguments()[0];
                //var deserializer = new ResponseProcessor(resultType);
                //return deserializer.DeserializeAsync(response.Content);

                return null;
            }

            throw new InvalidApiContractException($"Return type '{targetMethod.ReturnType}' is not supported.");
        }

        static void CheckForExpectedCode(EndpointModel endpointModel, HttpResponseMessage response)
        {
            bool is2xx = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            if(!is2xx)
                throw new UnexpectedStatusCodeException(response.StatusCode);
        }
    }

    interface IRequestProcessor
    {
        Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestMessage request);
    }
}
