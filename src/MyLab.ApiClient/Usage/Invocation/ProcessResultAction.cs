using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

class ProcessResultAction<TResult> : IProcessingAction
{
    readonly Action<TResult?, HttpStatusCode> _action;
    public CallDetailsPredicate Predicate { get;}
        
    public ProcessResultAction(Action<TResult?, HttpStatusCode> action, CallDetailsPredicate predicate)
    {
        Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }
        
    public async Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));

        var result = await callDetails.ReadContentAsync<TResult>();
        _action(result, callDetails.StatusCode);
    }
}