namespace MyLab.ApiClient
{
    interface IClientProxyStrategy
    {
        object Invoke(MethodDescription methodDescription, object[] args);
    }
}