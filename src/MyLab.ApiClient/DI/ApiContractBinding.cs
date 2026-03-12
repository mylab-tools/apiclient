using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Options;

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

    public bool TryGetOptions(ApiClientOptions opts, out ApiEndpointOptions? apiEndpointOpts)
    {
        apiEndpointOpts = opts.Endpoints
            .Where(ep => Keys.Contains(ep.Key))
            .Select(ep => ep.Value)
            .FirstOrDefault();

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