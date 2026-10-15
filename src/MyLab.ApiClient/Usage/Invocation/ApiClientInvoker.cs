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
    
    public IApiInvocationExpression<TContract> ForExpression(Expression<Func<TContract, Task>> expression)
    {
        return new ApiInvocationExpression<TContract>(expression, RequestProcLogic, new ApiInvocationBuildingState());
    }
}