using System.Collections.Generic;
using System.Linq;

namespace MyLab.ApiClient
{
    class ApiRequestFactoryContext
    {
        public MethodDescription Method { get; set; }

        public IParameterApplier[] ParameterAppliers { get; set; }

        public RequestFactoringSettings Settings { get; set; }
        
        public static ApiRequestFactoryContext Create(MethodDescription method, IApiRequestParameterValueProvider[] values, RequestFactoringSettings settings)
        {
            var ctx = new ApiRequestFactoryContext
            {
                Method = method,
                Settings = settings
            };


            if (values.Length !=
                method.Parameters.UrlParams.Count +
                method.Parameters.ContentParams.Count +
                method.Parameters.HeaderCollectionParams.Count +
                method.Parameters.HeaderParams.Count
            )
            {
                throw new ApiClientException("ParamAppliers number mismatch");
            }

            var callParams = new List<IParameterApplier>();

            callParams.AddRange(method.Parameters.UrlParams.Select(d =>
                new UrlParameterApplier(d, values[d.Position])));

            callParams.AddRange(method.Parameters.HeaderParams.Select(d =>
                new HeaderParameterApplier(d, values[d.Position])));

            callParams.AddRange(method.Parameters.HeaderCollectionParams.Select(d =>
                new HeaderCollectionParameterApplier(values[d.Position])));

            callParams.AddRange(method.Parameters.ContentParams.Select(d =>
                new ContentParameterApplier(d, values[d.Position], settings)));

            ctx.ParameterAppliers = callParams.ToArray();

            return ctx;
        }
    }
}