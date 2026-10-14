using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

class ProcessStatusCodeAction
(
    Action<HttpStatusCode> action,
    CallDetailsPredicate predicate
)
    : IProcessingAction
{
    public CallDetailsPredicate Predicate => predicate;

    public Task PerformAsync(CallDetails callDetails, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));
        action(callDetails.StatusCode);

        return Task.CompletedTask;
    }
}