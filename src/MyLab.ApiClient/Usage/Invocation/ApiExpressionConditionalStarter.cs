using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using MyLab.ApiClient.Problems;

namespace MyLab.ApiClient.Usage.Invocation;

class ApiExpressionConditionalStarter<TContract> 
(
    IApiInvocationExpressionFactory<TContract> expressionFactory,
    ApiInvocationBuildingState buildingState,
    CallDetailsPredicate callDetailsPredicate
)
    : IApiExpressionConditionalStarter<TContract>
{
    public IApiInvocationExpression<TContract> ProcessResult<TResult>(Action<TResult?, HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessResultAction<TResult>(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessStatusCode(Action<HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessStatusCodeAction(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessProblem(Action<ProblemDetails?, HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessResultAction<ProblemDetails>(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessValidationProblem(Action<ValidationProblemDetails?, HttpStatusCode> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var act = new ProcessResultAction<ValidationProblemDetails>(action, callDetailsPredicate);
        return AddProcAction(act);
    }

    public IApiInvocationExpression<TContract> ProcessResultAsync<TResult>(Func<TResult?, HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessResultAction<TResult>(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    public IApiInvocationExpression<TContract> ProcessStatusCodeAsync(Func<HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessStatusCodeAction(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    public IApiInvocationExpression<TContract> ProcessProblemAsync(Func<ProblemDetails?, HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessResultAction<ProblemDetails?>(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    public IApiInvocationExpression<TContract> ProcessValidationProblemAsync(Func<ValidationProblemDetails?, HttpStatusCode, Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var processingAction = new AsyncProcessResultAction<ValidationProblemDetails?>(func, callDetailsPredicate);
        return AddProcAction(processingAction);
    }

    IApiInvocationExpression<TContract> AddProcAction(IProcessingAction act)
    {
        var acts = buildingState.ProcessingActions != null
            ? [.. buildingState.ProcessingActions]
            : new List<IProcessingAction>();

        acts.Add(act);

        var newBuildState = buildingState with
        {
            ProcessingActions = new ReadOnlyCollection<IProcessingAction>(acts)
        };
        return expressionFactory.CreateExpression(newBuildState);
    }
}