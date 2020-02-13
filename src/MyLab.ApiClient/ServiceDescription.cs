using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient
{
    class ServiceDescription<TContract>
    {
        public string Url { get; }
        public IReadOnlyDictionary<int, MethodDescription> Methods { get; }
        
        public ServiceDescription(string url, IDictionary<int, MethodDescription> methods)
        {
            Url = url;
            Methods = new Dictionary<int, MethodDescription>(methods);
        }
        
        public static ServiceDescription<TContract> Create()
        {
            var t = typeof(TContract);

            if(!t.IsInterface)
                throw new ApiContractException($"Only interface contracts are supported. Actual type: '{t.FullName}'");
            
            var serviceAttr = t.GetCustomAttribute<ApiAttribute>();
            if(serviceAttr == null)
                throw new ApiContractException($"An API contract must be marked with attribute '{typeof(ApiAttribute).FullName}'");

            return new ServiceDescription<TContract>(
                serviceAttr.Url,
                t
                    .GetMethods()
                    .ToDictionary(m => m.MetadataToken, MethodDescription.Create));
        }
    }

    class MethodDescription
    {
        public string Url { get; }
        public HttpMethod HttpMethod { get; }
        
        public RequestParametersDescriptions Parameters { get; }
        
        public MethodDescription(string url, HttpMethod httpMethod,
            RequestParametersDescriptions parameters)
        {
            Url = url;
            HttpMethod = httpMethod;
            Parameters = parameters;
        }

        public static MethodDescription Create(MethodInfo method)
        {
            var ma = method.GetCustomAttribute<ApiMethodAttribute>();
            if(ma == null)
                throw new ApiContractException($"An API method must be marked with one of inheritors of '{typeof(ApiContractException).FullName}'");
            
            return new MethodDescription(ma.Url, ma.HttpMethod,
                    RequestParametersDescriptions.Create(method));
        }
    }
}
