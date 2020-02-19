using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient
{
    class MethodDescription
    {
        public string Url { get; }
        public HttpMethod HttpMethod { get; }

        public IReadOnlyList<HttpStatusCode> ExpectedStatusCodes { get; }
        
        public RequestParametersDescriptions Parameters { get; }
        
        public MethodDescription(string url, HttpMethod httpMethod,
            IEnumerable<HttpStatusCode> expectedCodes,
            RequestParametersDescriptions parameters)
        {
            Url = url;
            HttpMethod = httpMethod;
            Parameters = parameters;
            ExpectedStatusCodes= new List<HttpStatusCode>(expectedCodes);
        }

        public static MethodDescription Create(MethodInfo method)
        {
            var ma = method.GetCustomAttribute<ApiMethodAttribute>();
            if(ma == null)
                throw new ApiContractException($"An API method must be marked with one of inheritors of '{typeof(ApiContractException).FullName}'");
            
            return new MethodDescription(ma.Url, ma.HttpMethod,
                method
                    .GetCustomAttributes<ExpectedCodeAttribute>()
                    .Select(a => a.ExpectedCode),
                RequestParametersDescriptions.Create(method));
        }
    }
}