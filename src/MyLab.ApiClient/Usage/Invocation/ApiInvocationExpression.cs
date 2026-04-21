using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Tools;
using MyLab.ExpressionTools;

namespace MyLab.ApiClient.Usage.Invocation;

class ApiInvocationExpression<TContract>
(
    Expression<Func<TContract, Task>> expression,
    ReflectionRequestProcessingLogic processingLogic,
    ApiInvocationBuildingState buildingState
) : 
    IApiInvocationExpression<TContract>, 
    IApiInvocationExpressionFactory<TContract>
{
    public IApiExpressionConditionalStarter<TContract> When(HttpStatusCode code, params HttpStatusCode[] anotherCodes)
    {
        var predicateList = new List<StatusCodePredicate>
        {
            c => c == code
        };

        if (buildingState.HandledStatusPredicates != null)
        {
            predicateList.AddRange(buildingState.HandledStatusPredicates);
        }

        predicateList.AddRange
        (
            anotherCodes.Select(c => (StatusCodePredicate)(ac => ac == c))
        );

        var newState = buildingState with
        {
            HandledStatusPredicates = new ReadOnlyCollection<StatusCodePredicate>(predicateList)
        };

        var detailsPredicate = (CallDetailsPredicate)(d => predicateList.Any(p => p(d.StatusCode)));

        return new ApiExpressionConditionalStarter<TContract>(this, newState, detailsPredicate);
    }

    public IApiExpressionConditionalStarter<TContract> When1xx() => WhenXxx(100);

    public IApiExpressionConditionalStarter<TContract> When2xx() => WhenXxx(200);

    public IApiExpressionConditionalStarter<TContract> When3xx() => WhenXxx(300);

    public IApiExpressionConditionalStarter<TContract> When4xx() => WhenXxx(400);

    public IApiExpressionConditionalStarter<TContract> When5xx() => WhenXxx(500);

    public IApiInvocationExpression<TContract> ThrowIfAnotherStatusCode()
    {
        return CreateExpression(buildingState with { ThrowIfAnotherStatusCode = true });
    }

    public async Task<CallDetails> InvokeAsync(CancellationToken cancellationToken)
    {
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        if (!(expression.Body is MethodCallExpression mce))
            throw new ArgumentException("Must be lambda expression");

        var args = ExtractArgs(mce);

        var callDetails = await processingLogic.SendRequestAsync(mce.Method, args);

        await PostPreProcessAsync(callDetails, buildingState, cancellationToken);

        return callDetails;
    }

    public IApiInvocationExpression<TContract> CreateExpression(ApiInvocationBuildingState newBuildingState)
    {
        return new ApiInvocationExpression<TContract>(expression, processingLogic, newBuildingState);
    }

    ApiExpressionConditionalStarter<TContract> WhenXxx(int startCode)
    {
        var predicateList = new List<StatusCodePredicate>
        {
            c => (int)c >= startCode && (int)c < startCode + 100
        };

        if (buildingState.HandledStatusPredicates != null)
        {
            predicateList.AddRange(buildingState.HandledStatusPredicates);
        }

        var newState = buildingState with
        {
            HandledStatusPredicates = new ReadOnlyCollection<StatusCodePredicate>(predicateList)
        };

        var callDetailsPredicate = (CallDetailsPredicate)(d => (int)d.StatusCode >= startCode && (int)d.StatusCode < startCode + 100);

        return new ApiExpressionConditionalStarter<TContract>(this, newState, callDetailsPredicate);
    }

    static object[] ExtractArgs(MethodCallExpression mce)
    {
        var args = mce.Arguments
            .Select(a => a.GetValue<object>())
            .ToArray();
        return args;
    }

    async Task PostPreProcessAsync(CallDetails callDetails, ApiInvocationBuildingState state, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));

        if (state.ThrowIfAnotherStatusCode)
        {
            if (state.HandledStatusPredicates == null ||
                state.HandledStatusPredicates.All(p => !p(callDetails.StatusCode)))
            {
                throw new UnexpectedResponseStatusCodeException(callDetails.StatusCode);
            }
        }

        if (state.ProcessingActions != null)
        {
            foreach (var processingAction in state.ProcessingActions.Where(a => a.Predicate(callDetails)))
            {
                await processingAction.PerformAsync(callDetails, cancellationToken);
            }
        }
    }
}