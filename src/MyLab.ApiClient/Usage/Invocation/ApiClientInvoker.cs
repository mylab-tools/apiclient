using MyLab.ApiClient.Tools;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage.Invocation;

class ApiClientInvoker<TContract> : IApiClientInvoker<TContract>
{
    public ReflectionRequestProcessingLogic RequestProcLogic { get; }

    public ApiClientInvoker(ReflectionRequestProcessingLogic processingLogic)
    {
        RequestProcLogic = processingLogic ?? throw new ArgumentNullException(nameof(processingLogic));
    }
    
    /// <summary>
    /// Creates an API invocation expression based on the provided contract method expression.
    /// </summary>
    /// <param name="expression">
    /// An expression representing a method of the contract interface <typeparamref name="TContract"/> 
    /// that defines the API call to be invoked.
    /// </param>
    /// <returns>
    /// An object implementing <see cref="IApiInvocationExpression{TContract}"/> that allows further configuration 
    /// and execution of the API invocation.
    /// </returns>
    /// <typeparam name="TContract">
    /// The type of the contract interface that defines the API methods.
    /// </typeparam>
    public IApiInvocationExpression<TContract> ForExpression(Expression<Func<TContract, Task>> expression)
    {
        return new ApiInvocationExpression<TContract>(expression, RequestProcLogic, new ApiInvocationBuildingState());
    }
}