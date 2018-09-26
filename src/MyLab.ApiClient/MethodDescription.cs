using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class MethodDescription
    {
        public HttpMethod HttpMethod { get; }
        public string RelPath { get; set; }
        public Type ReturnType { get; }

        public IEnumerable<ParamDescription> Params { get; }

        public MethodDescription(HttpMethod httpMethod, Type returnType, IEnumerable<ParamDescription> paramDescriptions)
        {
            HttpMethod = httpMethod;
            ReturnType = returnType;

            var ps = new List<ParamDescription>(paramDescriptions);
            Params = ps.AsReadOnly();
        }

        public static MethodDescription Get(MethodInfo method)
        {
            var mAttr = method.GetCustomAttribute<ApiMethodAttribute>();
            if(mAttr == null)
                throw new ApiDescriptionException($"Method should be marked by {typeof(ApiMethodAttribute).FullName}");
            if(!typeof(Task).IsAssignableFrom(method.ReturnType) && 
               method.ReturnType != typeof(WebApiCall) &&  
               !(method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(WebApiCall<>)))
                    throw new ApiDescriptionException("Method should be asynchronously (returns Task or Task<>) or returns invocation (WebApiCall or WebApiCall<>)");

            var parameters = method.GetParameters()
                .Select(ParamDescription.Get)
                .ToArray();
            
            int bodyCount = parameters.Count(p => p.Place == ApiParamPlace.Body);

            if(bodyCount > 1)
                throw new ApiDescriptionException("Too many Body parameters. Only single one supported");

            var returnType = method.ReturnType == typeof(Task)
                ? typeof(void)
                : method.ReturnType.GenericTypeArguments[0];

            return  new MethodDescription(mAttr.HttpMethod, returnType, parameters)
            {
                RelPath = mAttr.RelPath
            };
        }
    }
}