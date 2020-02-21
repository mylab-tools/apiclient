using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Validates web api contract markup
    /// </summary>
    public static class MarkupValidator
    {
        /// <summary>
        /// Validates web api service contract
        /// </summary>
        public static IEnumerable<MarkupValidationIssuer> Validate(Type contractType)
        {
            var issues = new List<MarkupValidationIssuer>();

            var t = contractType;

            if(!t.IsInterface)
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = "A service contract must be an interface", 
                    Critical = true,
                    ServiceContract = t
                });

            var apiAttr = t.GetCustomAttribute<ApiAttribute>();

            if(apiAttr == null)
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = $"A service contract must be marked by {typeof(ApiAttribute).FullName}", 
                    Critical = true,
                    ServiceContract = t
                });
            else if(string.IsNullOrWhiteSpace(apiAttr.Url))
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = "A service base path should be specified",
                    ServiceContract = t
                });
            else
            {
                try
                {
                    new Uri(apiAttr.Url, UriKind.Relative);
                }
                catch (UriFormatException e)
                {
                    issues.Add(new MarkupValidationIssuer
                    {
                        Reason = $"Api url has invalid format: '{e.Message}'",
                        Critical = true,
                        ServiceContract = t
                    });
                }
            }

            foreach (var method in t.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                ValidateMethod(method, t, issues);
            }

            return issues;
        }

        private static void ValidateMethod(MethodInfo method, Type type, List<MarkupValidationIssuer> issues)
        {
            if(method.IsVirtual && !type.IsInterface)
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = "A service method must not be virtual",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });
            if (method.IsAbstract && !type.IsInterface)
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = "A service method must not be abstract",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });
            
            var mAttrs = method
                .GetCustomAttributes<ApiMethodAttribute>()
                .ToArray();

            if (mAttrs.Length > 1)
                issues.Add(new MarkupValidationIssuer
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
                    issues.Add(new MarkupValidationIssuer
                    {
                        Reason = "A method relative path should be specified",
                        ServiceContract = type,
                        Method = method
                    });
                else
                {
                    try
                    {
                        new Uri(mAttr.Url, UriKind.Relative);
                    }
                    catch (UriFormatException e)
                    {
                        issues.Add(new MarkupValidationIssuer
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
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = $"A service method must be market by inheritor of {typeof(ApiMethodAttribute).FullName}",
                    Critical = true,
                    ServiceContract = type,
                    Method = method
                });
            }

            var ps = method.GetParameters();

            if (ps.Count(p => p.GetCustomAttribute<ContentParameterAttribute>() != null) > 1)
                issues.Add(new MarkupValidationIssuer
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

        private static void ValidateParameter(ParameterInfo pi, MethodInfo method, Type type, List<MarkupValidationIssuer> issues)
        {
            if(pi.IsIn)
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = "Input parameters are not supported",
                    Critical = true,
                    ServiceContract = type,
                    Method = method,
                    Parameter = pi
                });
            if (pi.IsOut)
                issues.Add(new MarkupValidationIssuer
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
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = $"Method argument should be market by inheritor of'{typeof(ApiParameterAttribute)}'",
                    Critical = true,
                    ServiceContract = type,
                    Method = method,
                    Parameter = pi
                });
            if (pAttrs.Length > 1)
                issues.Add(new MarkupValidationIssuer
                {
                    Reason = "Only single api method parameter attribute per argument is supported",
                    Critical = true,
                    ServiceContract = type,
                    Method = method,
                    Parameter = pi
                });

        }
    }

    /// <summary>
    /// Contains markup validation issuer details
    /// </summary>
    public class MarkupValidationIssuer
    {
        public string Reason { get; set; }
        public bool Critical { get; set; }
        public Type ServiceContract { get; set; }
        public MethodInfo Method { get; set; }
        public ParameterInfo Parameter { get; set; }
    }
}
