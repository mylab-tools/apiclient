using System.Collections.ObjectModel;

namespace MyLab.ApiClient.Usage.Invocation;

record ApiInvocationBuildingState(
    ReadOnlyCollection<StatusCodePredicate>? HandledStatusPredicates = null,
    ReadOnlyCollection<IProcessingAction>? ProcessingActions = null,
    bool ThrowIfAnotherStatusCode = false
);