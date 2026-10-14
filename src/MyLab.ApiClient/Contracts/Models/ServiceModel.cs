using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;

namespace MyLab.ApiClient.Contracts.Models
{
    /// <summary>
    /// Represent API contract
    /// </summary>
    class ServiceModel
    {
        /// <summary>
        /// Gets the relative URL for all endpoints
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Gets the relative URL for all endpoints
        /// </summary>
        public IReadOnlyCollection<string>? Bindings { get; set; }

        /// <summary>
        /// Gets endpoint map by Method.MetadataToken
        /// </summary>
        public IReadOnlyDictionary<int, EndpointModel> Endpoints { get; }

        public ServiceModel(IReadOnlyDictionary<int, EndpointModel> endpoints)
        {
            Endpoints = endpoints ?? throw new ArgumentNullException(nameof(endpoints));
        }

        /// <summary>
        /// Creates <see cref="ServiceModel"/> from contract type
        /// </summary>
        public static ServiceModel FromContract(Type contractType, RequestFactoringSettings? settings = null)
        {
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            if (!contractType.IsInterface)
                throw new InvalidApiContractException("Api contract is not an interface");

            var methods = contractType.GetMethods()
                .ToDictionary
                (
                    m => m.MetadataToken,
                    m => EndpointModel.FromMethod(m, settings)
                );

            var bindings = new List<string>{ contractType.Name };
            
            if(contractType.FullName != null)
                bindings.Add(contractType.FullName);
            
            var aca = contractType.GetCustomAttribute<ApiContractAttribute>();
            
            if(aca?.Binding != null)
                bindings.Add(aca.Binding);

            return new ServiceModel(methods)
            {
                Url = aca?.Url,
                Bindings = bindings
            };
        }
    }
}
