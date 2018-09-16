using System;
using System.Reflection;

namespace MyLab.ApiClient
{
    class ParamDescription
    {
        public ApiParamPlace Place { get; }

        public string Name { get; }

        public ParamDescription(string name, ApiParamPlace place)
        {
            Name = name;
            Place = place;
        }

        public static ParamDescription Get(ParameterInfo parameterInfo)
        {
            var attr = parameterInfo.GetCustomAttribute<ApiParamAttribute>();
            if (attr == null)
                throw new ApiDescriptionException($"Parameter should be marked by {typeof(ApiParamAttribute).FullName}");

            return new ParamDescription(
                attr.Name ?? parameterInfo.Name,
                attr.Place
                );
        }
    }
}