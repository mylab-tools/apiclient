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

        public IReadOnlyList<ParamDescription> Params { get; private set; }

        public MethodDescription(HttpMethod httpMethod, IEnumerable<ParamDescription> paramDescriptions)
        {
            HttpMethod = httpMethod;

            var ps = new List<ParamDescription>(paramDescriptions);
            Params = ps.AsReadOnly();
        }

        public static MethodDescription Get(MethodInfo method)
        {
            var mAttr = method.GetCustomAttribute<ApiMethodAttribute>();
            if(mAttr == null)
                throw new ApiDescriptionException($"Method should be marked by {typeof(ApiMethodAttribute).FullName}");

            var parameters = method.GetParameters().Select(ParamDescription.Get);

            return  new MethodDescription(mAttr.HttpMethod, parameters)
            {
                RelPath = mAttr.RelPath
            };
        }
    }
}