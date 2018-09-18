using System;
using System.Reflection;
using System.Threading;

namespace MyLab.ApiClient
{
    class ParamDescription
    {
        public ApiParamPlace Place { get; }

        public string Name { get; }

        public bool IsCancellationToken { get; }

        public ParamDescription(string name, ApiParamPlace place, bool isCancellationToken)
        {
            Name = name;
            Place = place;
            IsCancellationToken = isCancellationToken;
        }

        public static ParamDescription Get(ParameterInfo parameterInfo)
        {
            var attr = parameterInfo.GetCustomAttribute<ApiParamAttribute>();
            bool isCancellationToken = parameterInfo.ParameterType == typeof(CancellationToken);


            if (attr == null && !isCancellationToken)
                throw new ApiDescriptionException($"Parameter should be marked by {typeof(ApiParamAttribute).FullName}");

            return isCancellationToken
                ? new ParamDescription(parameterInfo.Name, ApiParamPlace.Undefined, true)
                : new ParamDescription(attr.Name ?? parameterInfo.Name, attr.Place, false);
        }
    }
}