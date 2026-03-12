using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace MyLab.ApiClient.DI;

class ApiContractBinding
{
    readonly Type _contractType;

    public IReadOnlyCollection<string> Keys;
    
    public ApiContractBinding(Type contractType)
    {
        _contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        Keys = CreateContractBindingKeys();
    }

    public bool TryGetOptions(ApiClientOptions opts, out ApiEndpointOptions? apiEndpointOpts, out string? bindingKey)
    {
        foreach (var endpointKv in opts.Endpoints)
        {
            if (Keys.Contains(endpointKv.Key))
            {
                apiEndpointOpts = endpointKv.Value;
                bindingKey = endpointKv.Key;
                return true;
            }
        }

        apiEndpointOpts = null;
        bindingKey = null;

        return apiEndpointOpts != null;
    }

    string[] CreateContractBindingKeys()
    {
        var contractBindingKeys = new List<string>
        {
            _contractType.Name
        };

        if (_contractType.FullName != null)
        {
            contractBindingKeys.Add(_contractType.FullName!);
        }

        var contractAttr = _contractType.GetCustomAttribute<ApiContractAttribute>();
        if (contractAttr?.Binding != null)
        {
            contractBindingKeys.Add(contractAttr.Binding);
        }

        return contractBindingKeys.ToArray();
    }
}