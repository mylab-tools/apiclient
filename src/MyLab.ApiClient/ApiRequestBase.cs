using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides abilities to tune and send request
    /// </summary>
    public class ApiRequestBase<TDetails>
        where TDetails : CallDetails, new()
    {
        /// <summary>
        /// Gets request modifiers collection
        /// </summary>
        public List<IRequestMessageModifier> RequestModifiers { get; }
            = new List<IRequestMessageModifier>();

        /// <summary>
        /// Contains expected response http status codes
        /// </summary>
        public List<HttpStatusCode> ExpectedCodes { get; }
            = new List<HttpStatusCode>();

        private readonly string _baseUrl;
        private readonly MethodDescription _methodDescription;
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly IReadOnlyList<IParameterApplier> _paramAppliers;

        internal ApiRequestBase(
            string baseUrl,
            MethodDescription methodDescription, 
            IEnumerable<IParameterApplier> paramAppliers,
            IHttpClientProvider httpClientProvider)
        {
            if (paramAppliers == null) throw new ArgumentNullException(nameof(paramAppliers));
            _baseUrl = baseUrl;
            _methodDescription = methodDescription ?? throw new ArgumentNullException(nameof(methodDescription));
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
            _paramAppliers = paramAppliers.ToList().AsReadOnly();

            ExpectedCodes.AddRange(_methodDescription.ExpectedStatusCodes);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiRequestBase{TDetails}"/>
        /// </summary>
        protected ApiRequestBase(ApiRequestBase<TDetails> origin)
            :this(origin._baseUrl, origin._methodDescription, origin._paramAppliers, origin._httpClientProvider)
        {
            RequestModifiers.AddRange(origin.RequestModifiers);
        }

        /// <summary>
        /// Send request and return detailed information about operation
        /// </summary>
        public virtual async Task<TDetails> GetDetailedAsync(CancellationToken cancellationToken)
        {
            var resp = await SendRequestAsync(cancellationToken);

            var msgDumper = new HttpMessageDumper();
            var reqDump = await msgDumper.Dump(resp.Request);
            var respDump = await msgDumper.Dump(resp.Response);
            var isUnexpectedStatusCode = await IsStatusCodeUnexpectedAsync(resp.Response, false);

            var resDetails =  new TDetails
            {
                RequestMessage = resp.Request,
                ResponseMessage = resp.Response,
                RequestDump = reqDump,
                ResponseDump = respDump,
                StatusCode = resp.Response.StatusCode,
                IsUnexpectedStatusCode = isUnexpectedStatusCode
            };

            return resDetails;
        }

        protected async Task<HttpResponseMessage> GetResponseAsync(CancellationToken cancellationToken)
        {
            var resp = await SendRequestAsync(cancellationToken);

            await IsStatusCodeUnexpectedAsync(resp.Response, true);

            return resp.Response;
        }

        async Task<(HttpResponseMessage Response, HttpRequestMessage Request)> SendRequestAsync(CancellationToken cancellationToken)
        {
            Uri addr;

            try
            {
                addr = new Uri((_baseUrl?.TrimEnd('/') ?? "") + "/" + _methodDescription.Url, UriKind.RelativeOrAbsolute);
            }
            catch (Exception e)
            {
                e.Data.Add("baseUrl", _baseUrl);
                e.Data.Add("methodUrl", _methodDescription.Url);

                throw;
            }

            var reqMsg = new HttpRequestMessage
            {
                Method = _methodDescription.HttpMethod,
                RequestUri = addr
            };

            ApplyParameters(reqMsg);

            ApplyModifiers(reqMsg);

            HttpResponseMessage response;
            HttpClient httpClient = null;

            try
            {
                httpClient = _httpClientProvider.Provide();
                response = await httpClient.SendAsync(reqMsg, cancellationToken);
            }
            catch (Exception e)
            {
                e.Data.Add("httpClient is null", httpClient == null);

                if (httpClient != null) 
                {
                    e.Data.Add("httpClient.BaseAddress", httpClient.BaseAddress);
                }

                throw;
            }

            return (response, reqMsg);
        }

        private void ApplyParameters(HttpRequestMessage reqMsg)
        {
            foreach (var pApplier in _paramAppliers)
            {
                pApplier.Apply(reqMsg);
            }
        }

        private void ApplyModifiers(HttpRequestMessage reqMsg)
        {
            foreach (var requestModifier in RequestModifiers)
            {
                try
                {
                    requestModifier?.Modify(reqMsg);
                }
                catch (Exception e)
                {
                    throw new ApiClientException("An error occured while request modifying", e);
                }
            }
        }

        private async Task<bool> IsStatusCodeUnexpectedAsync(HttpResponseMessage response, bool throwIfTrue)
        {
            if (response.StatusCode != HttpStatusCode.OK &&
                !ExpectedCodes.Contains(response.StatusCode))
            {
                var contentString = await GetMessageFromResponseContentAsync(response.Content);
                string msg = !string.IsNullOrWhiteSpace(contentString) 
                    ? contentString
                    : response.ReasonPhrase;

                if(throwIfTrue)
                    throw new ResponseCodeException(response.StatusCode, msg);
                
                return true;
            }

            return false;
        }

        private async Task<string> GetMessageFromResponseContentAsync(HttpContent responseContent)
        {
            var contentStream = await responseContent.ReadAsStreamAsync();

            using var rdr = new StreamReader(contentStream);

            var buff = new char[1024];
            var read = await rdr.ReadBlockAsync(buff, 0, 1024);

            return new string(buff, 0, read);
        }
    }
}