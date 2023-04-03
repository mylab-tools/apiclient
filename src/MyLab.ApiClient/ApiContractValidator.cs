using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Validates web api contract
    /// </summary>
    public class ApiContractValidator
    {
        /// <summary>
        /// Validates web api service contract
        /// </summary>
        public ApiContractValidationResult Validate(Type contractType)
        {
            var issues = new List<ApiContractValidationIssuer>();

            var t = contractType;

            if(!t.IsInterface)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "A service contract must be an interface", 
                    Critical = true,
                    ServiceContract = t
                });

            var apiAttr = t.GetCustomAttribute<ApiAttribute>();

            if(apiAttr == null)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = $"A service contract must be marked by {typeof(ApiAttribute).FullName}", 
                    Critical = true,
                    ServiceContract = t
                });
            else
            {
                if (!string.IsNullOrWhiteSpace(apiAttr.Url))
                {
                    try
                    {
                        new Uri(apiAttr.Url, UriKind.Relative);
                    }
                    catch (UriFormatException e)
                    {
                        issues.Add(new ApiContractValidationIssuer
                        {
                            Reason = $"Api url has invalid format: '{e.Message}'",
                            Critical = true,
                            ServiceContract = t
                        });
                    }
                }
            }

            foreach (var method in t.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                ValidateMethod(method, t, issues);
            }

            return new ApiContractValidationResult(issues);
        }

        private static void ValidateMethod(MethodInfo method, Type type, List<ApiContractValidationIssuer> issues)
        {
            if(method.IsVirtual && !type.IsInterface)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "A service method must not be virtual",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });
            if (method.IsAbstract && !type.IsInterface)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "A service method must not be abstract",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });
            if(method.ReturnType != typeof(Task) && (!method.ReturnType.IsGenericType || method.ReturnType.GetGenericTypeDefinition() != typeof(Task<>)))
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "A service method must be async",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });
            var mAttrs = method
                .GetCustomAttributes<ApiMethodAttribute>()
                .ToArray();

            if (mAttrs.Length > 1)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "Only single api method attribute per method is supported",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });

            var mAttr = mAttrs.FirstOrDefault();

            if (mAttr != null)
            {
                if (string.IsNullOrWhiteSpace(mAttr.Url))
                {
                    //issues.Add(new ApiContractValidationIssuer
                    //{
                    //    Reason = "A method relative path should be specified",
                    //    ServiceContract = type,
                    //    Request = method
                    //});
                }
                else
                {
                    try
                    {
                        new Uri(mAttr.Url, UriKind.Relative);
                    }
                    catch (UriFormatException e)
                    {
                        issues.Add(new ApiContractValidationIssuer
                        {
                            Reason = $"Api method url has invalid format: '{e.Message}'",
                            Critical = true,
                            ServiceContract = type,
                            Method = method
                        });
                    }
                }
            }
            else
            {
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = $"A service method must be market by inheritor of {typeof(ApiMethodAttribute).FullName}",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });
            }

            var ps = method.GetParameters();

            if (ps.Count(p => p.GetCustomAttribute<ContentParameterAttribute>() != null) > 1)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "Only single content argument is supported",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });

            foreach (var pi in ps)
            {
                ValidateParameter(pi, method, type, issues);
            }
        }

        private static void ValidateParameter(ParameterInfo pi, MethodInfo method, Type type, List<ApiContractValidationIssuer> issues)
        {
            if(pi.IsIn)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "Input parameters are not supported",
                    Critical = true,
                    ServiceContract = type,
                    Method = method,
                    Parameter = pi
                });
            if (pi.IsOut)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "Output parameters are not supported",
                    Critical = true,
                    ServiceContract = type,
                    Method = method,
                    Parameter = pi
                });

            var pAttrs = pi
                .GetCustomAttributes<ApiParameterAttribute>()
                .ToArray();

            if (pAttrs.Length == 0)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = $"Request argument should be market by inheritor of'{typeof(ApiParameterAttribute)}'",
                    Critical = true,
                    ServiceContract = type,
                    Method = method,
                    Parameter = pi
                });
            if (pAttrs.Length > 1)
                issues.Add(new ApiContractValidationIssuer
                {
                    Reason = "Only single api method parameter attribute per argument is supported",
                    Critical = true,
                    ServiceContract = type,
                    Method = method,
                    Parameter = pi
                });

            var attrValIssue = pAttrs.FirstOrDefault()?.ValidateParameter(pi);
            if (attrValIssue != null)
            {
                attrValIssue.Method = method;
                attrValIssue.Parameter = pi;
                attrValIssue.ServiceContract = type;
                issues.Add(attrValIssue);
            }
        }
    }

    /// <summary>
    /// Contains validation issuer details
    /// </summary>
    public class ApiContractValidationIssuer
    {
        public string Reason { get; set; }
        public bool Critical { get; set; }
        public Type ServiceContract { get; set; }
        public MethodInfo Method { get; set; }
        public ParameterInfo Parameter { get; set; }
    }

    public class ApiContractValidationResult : ReadOnlyCollection<ApiContractValidationIssuer>
    {
        public bool Success => Count == 0;

        /// <summary>
        /// Initializes a new instance of <see cref="ApiContractValidationResult"/>
        /// </summary>
    public ApiContractValidationResult(IEnumerable<ApiContractValidationIssuer> issues)
        :base(new List<ApiContractValidationIssuer>(issues))
        {
            
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("===== Validation Result =====");

            if (Count == 0)
            {
                sb.AppendLine("<empty>");
            }
            else
            {
                foreach (var i in this)
                {
                    sb.AppendLine("");
                    sb.AppendLine(i.Reason + (i.Critical ? " (critical)" : " (warning)"));
                    sb.AppendLine("\t" + i.ServiceContract.FullName);
                    if (i.Method != null)
                        sb.AppendLine("\t\t" + i.Method.Name);
                    if (i.Parameter != null)
                        sb.AppendLine("\t\t" + i.Parameter.Name);
                }
            }

            return sb.ToString();
        }
    }
}
