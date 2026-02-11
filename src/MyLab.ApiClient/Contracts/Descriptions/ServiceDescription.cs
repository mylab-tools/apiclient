using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Descriptions
{
    /// <summary>
    /// Describes API contract
    /// </summary>
    class ServiceDescription
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
        public IReadOnlyDictionary<int, EndpointDescription> Endpoints { get; }

        public ServiceDescription(IReadOnlyDictionary<int, EndpointDescription> endpoints)
        {
            Endpoints = endpoints ?? throw new ArgumentNullException(nameof(endpoints));
        }

        /// <summary>
        /// Creates <see cref="ServiceDescription"/> from contract type
        /// </summary>
        public static ServiceDescription FromContract(Type contractType, RequestFactoringSettings? settings = null)
        {
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            if (!contractType.IsInterface)
                throw new InvalidApiContractException("Api contract is not an interface");

            var methods = contractType.GetMethods()
                .ToDictionary
                (
                    m => m.MetadataToken,
                    m => EndpointDescription.FromMethod(m, settings)
                );

            var bindings = new List<string>{ contractType.Name };
            
            if(contractType.FullName != null)
                bindings.Add(contractType.FullName);
            
            var aca = contractType.GetCustomAttribute<ApiContractAttribute>();
            
            if (aca == null)
                throw new InvalidApiContractException("ApiContract attribute not found");
            
            if(aca.Binding != null)
                bindings.Add(aca.Binding);

            return new ServiceDescription(methods)
            {
                Url = aca?.Url,
                Bindings = bindings
            };
        }
    }
}
