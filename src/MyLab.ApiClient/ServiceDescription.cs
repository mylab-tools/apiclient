using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
}
