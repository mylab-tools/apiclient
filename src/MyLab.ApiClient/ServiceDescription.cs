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
        
        ServiceDescription(string url, IDictionary<int, MethodDescription> methods)
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
        public string HttpMethod { get; }
        
        public IReadOnlyList<InputParameterDescription> Parameters { get; }
        
        MethodDescription(string url, string httpMethod,
            IEnumerable<InputParameterDescription> parameters)
        {
            Url = url;
            HttpMethod = httpMethod;
            Parameters = parameters.ToArray();
        }

        public static MethodDescription Create(MethodInfo method)
        {
            var ma = method.GetCustomAttribute<ApiMethodAttribute>();
            if(ma == null)
                throw new ApiContractException($"An API method must be marked with one of inheritors of '{typeof(ApiContractException).FullName}'");
            
            return new MethodDescription(ma.Url, ma.HttpMethod,
                method
                    .GetParameters()
                    .Select(InputParameterDescription.Create)
            );
        }
    }

    class InputParameterDescription
    {
        public string Name { get; }
        public IInputParameterFormatter Formatter { get; }
        public IInputParameterInjector Injector { get; }
        private InputParameterDescription(string name, IInputParameterInjector injector, IInputParameterFormatter formatter)
        {
            Name = name;
            Injector = injector;
            Formatter = formatter;
        }

        public static InputParameterDescription Create(ParameterInfo p)
        {
            var pa = p.GetCustomAttribute<ApiInputParameterAttribute>();
            if(pa == null)
                throw new ApiContractException($"An API method parameter must be marked with one of inheritors of '{typeof(ApiInputParameterAttribute)}'");
            
            return new InputParameterDescription(p.Name, pa.Injector,pa.Formatter);
        }
    }
}
