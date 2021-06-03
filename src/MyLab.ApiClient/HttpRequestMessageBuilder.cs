using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MyLab.ApiClient
{
    class HttpRequestMessageBuilder
    {
        private readonly MethodDescription _methodDescription;
        private readonly string _baseAddr;
        private readonly List<IParameterApplier> _paramAppliers = new List<IParameterApplier>();
        private readonly List<IRequestMessageModifier> _reqModifiers = new List<IRequestMessageModifier>();

        public HttpRequestMessageBuilder(string baseAddr, MethodDescription methodDescription)
        {
            _methodDescription = methodDescription;
            _baseAddr = baseAddr;
        }

        public void AddParameters(IEnumerable<IParameterApplier> parameterAppliers)
        {
            _paramAppliers.AddRange(parameterAppliers);
        }

        public void AddModifications(IEnumerable<IRequestMessageModifier> requestModifiers)
        {
            _reqModifiers.AddRange(requestModifiers);
        }

        public HttpRequestMessage Build()
        {
            Uri addr;

            try
            {
                string urlStart = "";
                string delimiter = "";

                if (!string.IsNullOrWhiteSpace(_baseAddr))
                {
                    urlStart = _baseAddr?.TrimEnd('/');

                    if (!string.IsNullOrWhiteSpace(_methodDescription.Url))
                    {
                        delimiter = "/";
                    }
                }

                addr = new Uri(urlStart + delimiter + _methodDescription.Url, UriKind.RelativeOrAbsolute);
            }
            catch (Exception e)
            {
                e.Data.Add("baseUrl", _baseAddr);
                e.Data.Add("methodUrl", _methodDescription.Url);

                throw;
            }

            var reqMsg = new HttpRequestMessage
            {
                Method = _methodDescription.HttpMethod,
                RequestUri = addr
            };

            foreach (var pApplier in _paramAppliers)
            {
                try
                {
                    pApplier.Apply(reqMsg);
                }
                catch (Exception e)
                {
                    throw new ApiClientException("An error occurred while request parameter applying", e);
                }
            }

            foreach (var requestModifier in _reqModifiers)
            {
                try
                {
                    requestModifier?.Modify(reqMsg);
                }
                catch (Exception e)
                {
                    throw new ApiClientException("An error occurred while request modifying", e);
                }
            }

            return reqMsg;
        }
    }
}
