using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

class AsyncProcessStatusCodeAction
(
    Func<HttpStatusCode, Task> func,
    CallDetailsPredicate predicate
)
    : IProcessingAction
{
    public CallDetailsPredicate Predicate => predicate;

    public async Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));
        await func(callDetails.StatusCode);
    }
}