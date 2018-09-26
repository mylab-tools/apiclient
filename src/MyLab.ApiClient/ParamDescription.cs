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
        public int Position { get; }
        public string MimeType { get; set; }

        public ParamDescription(
            string name, 
            ApiParamPlace place, 
            bool isCancellationToken,
            int position)
        {
            Name = name;
            Place = place;
            IsCancellationToken = isCancellationToken;
            Position = position;
        }

        public static ParamDescription Get(ParameterInfo parameterInfo)
        {
            var attr = parameterInfo.GetCustomAttribute<BaseParamAttribute>();
            bool isCancellationToken = parameterInfo.ParameterType == typeof(CancellationToken);
            
            if (attr == null)
            {
                if (!isCancellationToken)
                    throw new ApiDescriptionException($"Parameter should be marked by inheritor of {typeof(BaseParamAttribute).FullName}");
            }
            else
            {
                if (attr.Place == ApiParamPlace.Undefined)
                    throw new ApiDescriptionException("Parameter place should not be Undefined");
            }

            if (isCancellationToken)
            {
                return new ParamDescription(parameterInfo.Name, ApiParamPlace.Undefined, true, parameterInfo.Position);
            }

            if (attr is ReqBodyAttribute reqBodyAttr)
            {
                reqBodyAttr.Validate(parameterInfo.ParameterType);

                return new ParamDescription(attr.Name ?? parameterInfo.Name, attr.Place, false, parameterInfo.Position)
                {
                    MimeType = reqBodyAttr.MimeType
                };
            }

            return new ParamDescription(attr.Name ?? parameterInfo.Name, attr.Place, false, parameterInfo.Position);
        }
    }
}