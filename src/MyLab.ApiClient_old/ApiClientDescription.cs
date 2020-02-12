using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace MyLab.ApiClient
{
    class ApiClientDescription
    {
        readonly Dictionary<int, MethodDescription> _methods;

        public string RelPath { get; }

        public ApiClientDescription(string relPath, IDictionary<int, MethodDescription> methods)
        {
            RelPath = relPath;
            _methods= new Dictionary<int, MethodDescription>(methods);
        }

        public MethodDescription GetMethod(int methodMetadataToken)
        {
            if(!_methods.TryGetValue(methodMetadataToken, out var md)) 
                throw new ApiDescriptionException("Method description not found");

            return md;
        }

        public static ApiClientDescription Get(Type contract)
        {
            if (!contract.IsInterface)
                throw new ApiDescriptionException("Only interface is supported");

            var apiAttr = contract.GetCustomAttribute<ApiAttribute>();
            if (apiAttr == null)
                throw new ApiDescriptionException($"Api contract should be marked by {typeof(ApiAttribute).FullName}");

            var methods = contract.GetMethods()
                .Select(m => new { Token = m.MetadataToken, Desc = MethodDescription.Get(m) })
                .ToDictionary(p => p.Token, p => p.Desc);

            return new ApiClientDescription(apiAttr.RelPath, methods);
        }
    }
}
