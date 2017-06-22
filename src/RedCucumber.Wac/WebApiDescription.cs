using System;
using System.Linq;
using System.Reflection;

namespace RedCucumber.Wac
{
    struct WebApiDescription
    {
        public string BaseUrl { get; set; }
        public WebApiMethodDescriptions Methods { get; set; }

        public static WebApiDescription Create(Type contractType)
        {
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            
            var wasAttr = contractType.GetTypeInfo().GetCustomAttribute<WebApiServiceAttribute>();

            return new WebApiDescription
            {
                BaseUrl = wasAttr?.SubPath,
                Methods = new WebApiMethodDescriptions(
                    contractType.GetTypeInfo().FindMembers(
                            MemberTypes.Method,
                            BindingFlags.Instance | BindingFlags.Public,
                            (info, criteria) => info.GetCustomAttribute<ServiceEndpointAttribute>() != null,
                            null)
                        .Cast<MethodInfo>()
                        .Select(WebApiMethodDescription.Create)
                )
            };
        }
    }
}