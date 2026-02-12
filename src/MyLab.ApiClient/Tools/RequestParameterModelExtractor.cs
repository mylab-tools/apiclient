using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyLab.ApiClient.Tools
{
    static class RequestParameterModelExtractor
    {
        /// <summary>
        /// Extract <see cref="IRequestParameterModel"/> from reflection method
        /// </summary>
        public static IEnumerable<IRequestParameterModel> FromMethod(MethodInfo mi, RequestFactoringSettings? settings = null)
        {
            if (mi == null) throw new ArgumentNullException(nameof(mi));

            var allParams = new List<IRequestParameterModel>();

            var props = mi.GetParameters();

            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];

                var attr = GetParamAttr(p);

                switch (attr)
                {
                    case UrlParameterAttribute urlA:
                        allParams.Add(new UrlParameterModel(i, urlA.Name ?? p.Name!, urlA.UrlModifier) { Settings = settings });
                        break;
                    case HeaderAttribute hdrA:
                        allParams.Add(new HeaderParameterModel(i, hdrA.Name ?? p.Name!) { Settings = settings });
                        break;
                    case HeaderCollectionAttribute:
                        allParams.Add(new HeaderCollectionParameterModel(i) { Settings = settings });
                        break;
                    case ContentParameterAttribute contA:
                        allParams.Add(new ContentParameterModel(i, contA.HttpContentFactory) { Settings = settings });
                        break;
                    default:
                        throw new InvalidApiContractException($"Request attribute type '{attr.GetType().FullName}' is not supported");
                }
            }

            return allParams;
        }

        static ApiParameterAttribute GetParamAttr(ParameterInfo p)
        {
            var pas = p.GetCustomAttributes<ApiParameterAttribute>().ToArray();
            if (pas.Length == 0)
                throw new InvalidApiContractException($"An API method parameter must be marked with one of inheritors of '{typeof(ApiParameterAttribute)}'");
            if (pas.Length > 1)
                throw new InvalidApiContractException($"Only single '{typeof(ApiParameterAttribute)}' attribute is supported");

            return pas.Single();
        }
    }
}
