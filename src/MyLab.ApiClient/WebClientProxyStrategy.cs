using System;
using System.Reflection;

namespace MyLab.ApiClient
{
    class WebClientProxyStrategy : IClientProxyStrategy
    {
        public object Invoke(MethodInfo method, ApiClientDescription description, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}