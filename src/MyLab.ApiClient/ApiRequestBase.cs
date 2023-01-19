using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
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
        private readonly string _baseUrl;
        private readonly ApiRequestFactoryContext _apiRequestFactoryContext;
        private readonly IHttpClientProvider _httpClientProvider;

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
        
        internal ApiRequestBase(
            string baseUrl,
            ApiRequestFactoryContext apiRequestFactoryContext,
            IHttpClientProvider httpClientProvider)
        {
            _baseUrl = baseUrl;
            
            _apiRequestFactoryContext = apiRequestFactoryContext ?? throw new ArgumentNullException(nameof(apiRequestFactoryContext));

            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));

            ExpectedCodes.AddRange(apiRequestFactoryContext.Method.ExpectedStatusCodes);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiRequestBase{TDetails}"/>
        /// </summary>
        protected ApiRequestBase(ApiRequestBase<TDetails> origin)
            :this(origin._baseUrl, origin._apiRequestFactoryContext, origin._httpClientProvider)
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
            var isUnexpectedStatusCode = IsStatusCodeUnexpectedAsync(resp.Response);

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

            if (IsStatusCodeUnexpectedAsync(resp.Response))
                throw await ResponseCodeException.FromResponseMessage(resp.Response);

            return resp.Response;
        }

        async Task<(HttpResponseMessage Response, HttpRequestMessage Request)> SendRequestAsync(CancellationToken cancellationToken)
        {
            var reqMsgBuilder = new HttpRequestMessageBuilder(_baseUrl, _apiRequestFactoryContext.Method);
            reqMsgBuilder.AddParameters(_apiRequestFactoryContext.ParameterAppliers);
            reqMsgBuilder.AddModifications(RequestModifiers);

            var reqMsg = reqMsgBuilder.Build();

            HttpResponseMessage response;
            HttpClient httpClient = null;

            try
            {
                httpClient = _httpClientProvider.Provide();
                response = await httpClient.SendAsync(reqMsg, cancellationToken);
            }
            catch (Exception e)
            {
                if(!e.Data.Contains("httpClient is null"))
                    e.Data.Add("httpClient is null", httpClient == null);

                if (httpClient != null && !e.Data.Contains("httpClient.BaseAddress")) 
                    e.Data.Add("httpClient.BaseAddress", httpClient.BaseAddress);

                if (!e.Data.Contains("reqMsg.RequestUri"))
                    e.Data.Add("reqMsg.RequestUri", reqMsg.RequestUri);
                if (!e.Data.Contains("reqMsg.Method"))
                    e.Data.Add("reqMsg.Method", reqMsg.Method);
                if (!e.Data.Contains("reqMsg.Headers"))
                    e.Data.Add("reqMsg.Headers", reqMsg.Headers);

                throw;
            }

            return (response, reqMsg);
        }

        private bool IsStatusCodeUnexpectedAsync(HttpResponseMessage response)
        {
            return response.StatusCode != HttpStatusCode.OK &&!ExpectedCodes.Contains(response.StatusCode);
        }
    }
}