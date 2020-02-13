using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides abilities to send requests to web API
    /// </summary>
    /// <typeparam name="TContract">API contract</typeparam>
    public class ApiClient<TContract>
    {
        private readonly ServiceDescription<TContract> _description;
        private readonly IHttpClientProvider _httpClientProvider;

        private ApiClient(ServiceDescription<TContract> description, IHttpClientProvider httpClientProvider)
        {
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
        }
        
        /// <summary>
        /// Creates API client based on http client factory with specified service name
        /// </summary>
        public static ApiClient<TContract> Create(IHttpClientProvider httpClientProvider)
        {
            return new ApiClient<TContract>(
                ServiceDescription<TContract>.Create(),
                httpClientProvider);
        }

        public IApiRequest Request(Expression<Action<TContract>> serviceCallExpr)
        {
            if(!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");
            
            var exprParams = GetParametersForExpression(mExpr);
            
            return new ApiRequestWithoutReturnValue(
                _description.Url, 
                exprParams.MethodDescription,
                exprParams.Appliers,
                _httpClientProvider);
        }

        public IApiRequest<TRes> Request<TRes>(Expression<Func<TContract, TRes>> serviceCallExpr)
        {
            if(!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");
            
            var exprParams = GetParametersForExpression(mExpr);
            
            return new ApiRequestWithReturnValue<TRes>(
                _description.Url, 
                exprParams.MethodDescription,
                exprParams.Appliers,
                _httpClientProvider);
        }

        (MethodDescription MethodDescription, IEnumerable<IParameterApplier> Appliers) GetParametersForExpression(MethodCallExpression serviceCallExpr)
        {
            if(!(serviceCallExpr is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");
            if(!_description.Methods.TryGetValue(mExpr.Method.MetadataToken, out var mDesc))
                throw new ApiClientException("Specified method description not found");

            var args = mExpr.Arguments;
            
            if(args.Count != mDesc.Parameters.UrlParams.Count+ mDesc.Parameters.ContentParams.Count+ mDesc.Parameters.HeaderParams.Count)
                throw new ApiClientException("ParamAppliers number mismatch");
            
            var callParams = new List<IParameterApplier>();

            callParams.AddRange(mDesc.Parameters.UrlParams.Select(d => 
                new UrlParameterApplier(
                    d, 
                    new DefaultApiRequestParameterValueProvider(args[d.Position])
                    )));

            callParams.AddRange(mDesc.Parameters.HeaderParams.Select(d =>
                new HeaderParameterApplier(
                    d,
                    new DefaultApiRequestParameterValueProvider(args[d.Position])
                )));

            callParams.AddRange(mDesc.Parameters.ContentParams.Select(d =>
                new ContentParameterApplier(
                    d,
                    new DefaultApiRequestParameterValueProvider(args[d.Position])
                )));

            return (mDesc, callParams);
        }
    }
}