using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyLab.ApiClient
{
    class ServiceDescription<TContract>
    {
        public string Url { get; private set; }
        public IReadOnlyDictionary<int, MethodDescription> Methods { get; private set; }
        
        public static ServiceDescription<TContract> Create()
        {
            var t = typeof(TContract);

            if(!t.IsInterface)
                throw new ApiContractException($"Only interface contracts are supported. Actual type: '{t.FullName}'");
            
            var serviceAttr = t.GetCustomAttribute<ApiAttribute>();
            if(serviceAttr == null)
                throw new ApiContractException($"An API contract must be marked with attribute '{typeof(ApiAttribute).FullName}'");

            return new ServiceDescription<TContract>
            {
                Url = serviceAttr.Url,
                Methods = t
                    .GetMethods()
                    .ToDictionary(m => m.MetadataToken, MethodDescription.Create)
            };
        }
    }

    class MethodDescription
    {
        public string Url { get; private set; }
        public string HttpMethod { get; private set; }
        
        public IReadOnlyDictionary<string, InputParameterDescription> Parameters { get; private set; }
        
        private MethodDescription()
        {
            
        }

        public static MethodDescription Create(MethodInfo method)
        {
            var ma = method.GetCustomAttribute<ApiMethodAttribute>();
            if(ma == null)
                throw new ApiContractException($"An API method must be marked with one of inheritors of '{typeof(ApiContractException).FullName}'");
            
            return new MethodDescription
            {
                Url = ma.Url,
                HttpMethod = ma.HttpMethod,
                Parameters = method
                    .GetParameters()
                    .ToDictionary(p => p.Name, InputParameterDescription.Create)
            };
        }
    }

    class InputParameterDescription
    {
        public IInputParameterFormatter Formatter { get; private set; }
        
        public IInputParameterInjector Injector { get; private set; }
        
        InputParameterDescription()
        {
            
        }

        public static InputParameterDescription Create(ParameterInfo p)
        {
            var pa = p.GetCustomAttribute<ApiInputParameterAttribute>();
            if(pa == null)
                throw new ApiContractException($"An API method parameter must be marked with one of inheritors of '{typeof(ApiInputParameterAttribute)}'");
            
            return new InputParameterDescription
            {
                Injector = pa.Injector,
                Formatter = pa.Formatter
            };
        }
    }
}
