using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient
{
    class MethodDescription
    {
        public HttpMethod HttpMethod { get; }

        public string RelPath { get; set; }

        private readonly Dictionary<string, ParamDescription> _params;

        public MethodDescription(HttpMethod httpMethod, IDictionary<string, ParamDescription> paramDescriptions)
        {
            HttpMethod = httpMethod;
            _params = new Dictionary<string, ParamDescription>(paramDescriptions);
        }

        public ParamDescription GetParameter(string paramName)
        {
            if (!_params.TryGetValue(paramName, out var md))
                throw new ApiDescriptionException("Method description not found");

            return md;
        }

        public static MethodDescription Get(MethodInfo method)
        {
            var mAttr = method.GetCustomAttribute<ApiMethodAttribute>();
            if(mAttr == null)
                throw new ApiDescriptionException($"Method should be marked by {typeof(ApiMethodAttribute).FullName}");

            var parameters = method.GetParameters()
                .Select(p => new { Name= p.Name, Desc = ParamDescription.Get(p)})
                .ToDictionary(p => p.Name, p => p.Desc);

            return  new MethodDescription(mAttr.HttpMethod, parameters)
            {
                RelPath = mAttr.RelPath
            };
        }
    }
}