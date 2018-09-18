using System;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient
{
    class WebClientProxyStrategy : IClientProxyStrategy
    {
        public object Invoke(MethodInfo method, ApiClientDescription description, object[] args)
        {
            //var client = new HttpClient();
            //client.
            throw new NotImplementedException();
        }
    }

    interface IHttpRequestInvoker
    {

    }
}