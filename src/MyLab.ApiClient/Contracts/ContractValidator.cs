using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using System;
using System.Linq;
using System.Reflection;
using MyLab.ApiClient.Contracts.Attributes.ForContract;

namespace MyLab.ApiClient.Contracts
{
    static class ContractValidator
    {
        public static void Validate(Type contract)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));

            if (!contract.IsInterface)
                throw new InvalidApiContractException("The contract must be interface");
            
            if(contract.GetCustomAttribute<ApiContractAttribute>() == null)
                throw new InvalidApiContractException("The contract must be decorated with ApiContractAttribute");

            var allMembers = contract.GetMembers();

            if (allMembers.Any(m => m.MemberType != MemberTypes.Method))
                throw new InvalidApiContractException("The contract must contains only methods");

            var methods = allMembers
                .Where(m => m.MemberType == MemberTypes.Method)
                .Cast<MethodInfo>()
                .ToArray();

            if (methods.Any(m => m.IsGenericMethod))
                throw new InvalidApiContractException("The generic methods are prohibited");

            if (methods.Any(m => m.GetCustomAttribute<ApiMethodAttribute>() == null))
                throw new InvalidApiContractException("All contract methods must be decorated with one of ApiMethodAttribute inheritors");

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                
                if (parameters.Any(p => p.IsOut || p.ParameterType.IsByRef))
                    throw new InvalidApiContractException("The contract method must not have ref or out parameters");

                foreach (var p in parameters)
                {
                    var apiAttr = p.GetCustomAttribute<ApiParameterAttribute>();
                    
                    if(apiAttr == null)
                        throw new InvalidApiContractException("All contract method parameters must be decorated with one of ApiParameterAttribute inheritors");
                }
            }
        }
    }
}
