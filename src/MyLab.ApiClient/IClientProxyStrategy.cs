using System.Reflection;

namespace MyLab.ApiClient
{
    interface IClientProxyStrategy
    {
        object Invoke(MethodInfo method, ApiClientDescription description, object[] args);
    }
}