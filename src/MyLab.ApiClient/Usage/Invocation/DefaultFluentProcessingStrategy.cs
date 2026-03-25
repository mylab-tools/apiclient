using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing;

namespace MyLab.ApiClient.Usage.Invocation;

class DefaultFluentProcessingStrategy : IFluentProcessingStrategy
{
    public async Task ProcessAsync(CallDetails callDetails, CallResultProcessingContext processingContext, CancellationToken cancellationToken)
    {
        if (callDetails == null) throw new ArgumentNullException(nameof(callDetails));

        if (processingContext.ThrowIfAnotherStatusCode)
        {
            if (processingContext.HandledStatusPredicates == null ||
                processingContext.HandledStatusPredicates.All(p => !p(callDetails.StatusCode)))
            {
                throw new UnexpectedResponseStatusCodeException(callDetails.StatusCode);
            }
        }

        if (processingContext.ProcessingActions != null)
        {
            foreach (var processingAction in processingContext.ProcessingActions.Where(a => a.Predicate(callDetails)))
            {
                await processingAction.PerformAsync(callDetails, cancellationToken);
            }
        }
    }
}