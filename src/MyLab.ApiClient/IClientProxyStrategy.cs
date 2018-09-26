using System.Reflection;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    interface IClientProxyStrategy
    {
        WebApiCall GetCall(MethodInfo method, ApiClientDescription description, object[] args);

        WebApiCall<TResult> GetCall<TResult>(MethodInfo method, ApiClientDescription description, object[] args);
    }
}