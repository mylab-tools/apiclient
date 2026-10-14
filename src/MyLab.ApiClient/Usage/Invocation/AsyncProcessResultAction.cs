using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

class AsyncProcessResultAction<TResult>
(
    Func<TResult?, HttpStatusCode, Task> func,
    CallDetailsPredicate predicate
)
    : IProcessingAction
{
    public CallDetailsPredicate Predicate => predicate;

    public async Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));

        var result = await callDetails.ReadContentAsync<TResult>();
        await func(result, callDetails.StatusCode);
    }
}