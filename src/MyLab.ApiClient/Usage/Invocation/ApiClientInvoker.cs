using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Tools;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MyLab.ExpressionTools;

namespace MyLab.ApiClient.Usage.Invocation
{
    /// <summary>
    /// Provides functionality to invoke methods on an API client contract.
    /// </summary>
    /// <typeparam name="TContract">
    /// The type of the API client contract that defines the interface for interacting with the API.
    /// </typeparam>
    public class ApiClientInvoker<TContract>
    {
        readonly ReflectionRequestProcessingLogic _processingLogic;

        ApiClientInvoker(ReflectionRequestProcessingLogic processingLogic)
        {
            _processingLogic = processingLogic ?? throw new ArgumentNullException(nameof(processingLogic));
        }

        /// <summary>
        /// Creates an instance of <see cref="ApiClientInvoker{TContract}"/> for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">
        /// The contract type that defines the API client interface.
        /// </typeparam>
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
            var sender = new ReflectionRequestProcessingLogic(serviceModel, requestProcessor, callDetailsFactory);
            
            return new ApiClientInvoker<TContract>(sender);
        }
        
        /// <summary>
        /// Invokes a method on the API client contract asynchronously.
        /// </summary>
        /// <param name="invocation">
        /// An expression representing the method invocation on the API client contract.
        /// The expression must be a lambda expression that specifies the method to call and its arguments.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an 
        /// <see cref="ApiClientInvocationResult"/> object, which provides details about the API call and its processing context.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="invocation"/> parameter is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="invocation"/> parameter is not a valid lambda expression representing a method call.
        /// </exception>
        public async Task<ApiClientInvocationResult> InvokeAsync(Expression<Func<TContract, Task>> invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            
            if(!(invocation.Body is MethodCallExpression mce))
            {
                throw new ArgumentException("Must be lambda expression");
            }

            var args = mce.Arguments
                .Select(a => a.GetValue<object>())
                .ToArray();

            var callDetails = await _processingLogic.SendRequestAsync(mce.Method, args);

            return new ApiClientInvocationResult(callDetails, new CallResultProcessingContext());
        }
    }
}
