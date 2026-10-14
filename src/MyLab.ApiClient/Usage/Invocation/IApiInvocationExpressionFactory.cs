namespace MyLab.ApiClient.Usage.Invocation;

interface IApiInvocationExpressionFactory<TContract>
{
    IApiInvocationExpression<TContract> CreateExpression(ApiInvocationBuildingState buildingState);
}