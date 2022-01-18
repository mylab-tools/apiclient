using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyLab.ApiClient
{
    class ServiceDescription
    {
        public string Url { get; }
        public IReadOnlyDictionary<int, MethodDescription> Methods { get; }
        
        public ServiceDescription(string url, IDictionary<int, MethodDescription> methods)
        {
            Url = url;
            Methods = new Dictionary<int, MethodDescription>(methods);
        }
        
        public static ServiceDescription Create(Type contractType)
        {
            var t = contractType;

            if(!t.IsInterface)
                throw new ApiContractException($"Only interface contracts are supported. Actual type: '{t.FullName}'");
            
            var serviceAttr = t.GetCustomAttribute<ApiAttribute>();
            if(serviceAttr == null)
                throw new ApiContractException($"An API contract must be marked with attribute '{typeof(ApiAttribute).FullName}'");

            return new ServiceDescription(
                serviceAttr.Url,
                t
                    .GetMethods()
                    .ToDictionary(m => m.MetadataToken, MethodDescription.Create));
        }

        public bool TryGetMethod(MethodInfo method, out MethodDescription mDesc)
        {
            return Methods.TryGetValue(method.MetadataToken, out mDesc);
        }

        public MethodDescription GetRequiredMethod(MethodInfo method)
        {
            if (!Methods.TryGetValue(method.MetadataToken, out var mDesc))
                throw new ApiClientException("Specified method description not found");
            return mDesc;
        }
    }
}
