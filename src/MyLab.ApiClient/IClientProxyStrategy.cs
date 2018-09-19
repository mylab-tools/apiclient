using System.Reflection;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    interface IClientProxyStrategy
    {
        WebApiInvocation GetInvocation(MethodInfo method, ApiClientDescription description, object[] args);

        WebApiInvocation<TResult> GetInvocation<TResult>(MethodInfo method, ApiClientDescription description, object[] args);
    }
}