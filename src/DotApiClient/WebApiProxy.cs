using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading.Tasks;

namespace DotAspectClient
{
    class WebApiProxy<T> : RealProxy
        where T : class 
    {
        private readonly WebApiDescription _webApiDescription;
        private readonly WebApiClientOptions _options;
        private readonly SupportedResponseProcessors _responseProcessors = new SupportedResponseProcessors();

        public WebApiProxy(
            WebApiDescription webApiDescription,
            WebApiClientOptions options)
            :base(typeof(T))
        {
            _webApiDescription = webApiDescription;
            _options = options;
        }

        public override IMessage Invoke(IMessage msg)
        {
            ReturnMessage responseMessage;
            Object response = null;
            Exception caughtException = null;

            try
            {
                var methodName = (string)msg.Properties["__MethodName"];
                var methodSignature = (Type[])msg.Properties["__MethodSignature"];
                var args = (object[])msg.Properties["__Args"];

                if (_webApiDescription.Methods != null)
                {
                    var methodInfo = typeof(T).GetMethod(methodName, methodSignature ?? Type.EmptyTypes);
                    var methodId = WebApiMethodDescription.CreateMethodId(methodName,
                        methodInfo.GetParameters().Select(p => p.Name));
                    WebApiMethodDescription methodDesc;
                    if (!_webApiDescription.Methods.TryGetValue(methodId, out methodDesc))
                        throw new PrepareRequestException("Method not found");

                    var rFact = new WebApiRequestFactory(methodDesc, _webApiDescription.BaseUrl)
                    {
                        Options = _options
                    };

                    var paramsNames = methodDesc.Parameters.Values
                        .OrderBy(p => p.Order)
                        .Select(p => p.MethodParameterName)
                        .ToArray();

                    var requestMessage = rFact.CreateMessage(new InvokeParameters(paramsNames, args));
                    var task = (_options?.RequestProcessor ?? new DefaultWebApiRequestProcessor())
                        .ProcessRequest(requestMessage);
                    
                    foreach (var responseProcessor in _responseProcessors)
                    {
                        if (responseProcessor.Predicate(methodInfo.ReturnType))
                        {
                            response = responseProcessor.GetResponse(task, methodInfo.ReturnType);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Store the caught exception
                caughtException = ex;
            }

            IMethodCallMessage message = msg as IMethodCallMessage;

            // Check if there is an exception
            if (caughtException == null)
            {
                // Return the response from the service
                responseMessage = new ReturnMessage(response, null, 0, null, message);
            }
            else
            {
                // Return the exception thrown by the service
                responseMessage = new ReturnMessage(caughtException, message);
            }

            // Return the response message
            return responseMessage;
        }
    }
}