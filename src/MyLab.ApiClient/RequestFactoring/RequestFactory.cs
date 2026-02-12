using System;
using System.Net.Http;
using System.Text;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Models;

namespace MyLab.ApiClient.RequestFactoring
{
    class RequestFactory
    {
        readonly ServiceDescription _serviceDesc;
        readonly EndpointDescription _epDesc;

        public RequestFactory(ServiceDescription serviceDesc, EndpointDescription epDesc)
        {
            _serviceDesc = serviceDesc;
            _epDesc = epDesc;
        }

        public HttpRequestMessage Create(object[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            
            if (_epDesc.Parameters.Count != parameters.Length)
            {
                throw new InvalidApiContractException(
                    $"Endpoint '{_epDesc.Url}' expects {_epDesc.Parameters.Count} parameters, but {parameters.Length} were provided.");
            }
            
            var request = new HttpRequestMessage { Method = _epDesc.HttpMethod };

            request.RequestUri = CombinePath();

            ApplyParameters(request, parameters);
            
            return request;
        }

        Uri CombinePath()
        {
            var sb = new StringBuilder();

            if (_serviceDesc.Url != null) sb.Append(_serviceDesc.Url);
            if(sb.Length != 0 && sb[^1] != '/') sb.Append("/");
            if (_epDesc.Url != null) sb.Append(_epDesc.Url);

            var strUrl = sb.ToString();

            return strUrl != string.Empty 
                ? new Uri(strUrl, UriKind.Relative)
                : new Uri("/", UriKind.Relative);
        }

        void ApplyParameters(HttpRequestMessage request, object[] parameters)
        {
            foreach (var paramDesc in _epDesc.Parameters)
            {
                var paramValue = parameters[paramDesc.Position];
                paramDesc.Apply(request, paramValue);
            }
        }
    }
}
