using System;
using System.Net.Http;
using System.Text;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Models;

namespace MyLab.ApiClient.RequestFactoring
{
    class RequestFactory
    {
        readonly ServiceModel _serviceDesc;
        readonly EndpointModel _epModel;

        public RequestFactory(ServiceModel serviceDesc, EndpointModel epModel)
        {
            _serviceDesc = serviceDesc;
            _epModel = epModel;
        }

        public HttpRequestMessage Create(object?[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            
            if (_epModel.Parameters.Count != parameters.Length)
            {
                throw new InvalidApiContractException(
                    $"Endpoint '{_epModel.Url}' expects {_epModel.Parameters.Count} parameters, but {parameters.Length} were provided.");
            }
            
            var request = new HttpRequestMessage { Method = _epModel.HttpMethod };

            request.RequestUri = CombinePath();

            ApplyParameters(request, parameters);
            
            return request;
        }

        Uri CombinePath()
        {
            var sb = new StringBuilder();

            if (_serviceDesc.Url != null) sb.Append(_serviceDesc.Url);
            if(sb.Length != 0 && sb[^1] != '/') sb.Append("/");
            if (_epModel.Url != null) sb.Append(_epModel.Url);

            var strUrl = sb.ToString();

            return strUrl != string.Empty 
                ? new Uri(strUrl, UriKind.Relative)
                : new Uri("/", UriKind.Relative);
        }

        void ApplyParameters(HttpRequestMessage request, object?[] parameters)
        {
            foreach (var paramDesc in _epModel.Parameters)
            {
                var paramValue = parameters[paramDesc.Position];
                paramDesc.Apply(request, paramValue);
            }
        }
    }
}
