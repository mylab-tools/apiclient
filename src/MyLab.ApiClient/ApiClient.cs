using System;
using System.Collections.Generic;
using System.Data;
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
        private ServiceDescription<TContract> _description;

        private IHttpClientFactory _httpClientFactory;

        private ApiClient(ServiceDescription<TContract> description, IHttpClientFactory httpClientFactory)
        {
            _description = description;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Creates API client based on http client factory with default service name
        /// </summary>
        public static ApiClient<TContract> Create(IHttpClientFactory clientFactory)
        {
            return Create(clientFactory, Options.DefaultName);
        }
        
        /// <summary>
        /// Creates API client based on http client factory with specified service name
        /// </summary>
        public static ApiClient<TContract> Create(IHttpClientFactory clientFactory, string serviceName)
        {
            if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));

            return new ApiClient<TContract>(
                ServiceDescription<TContract>.Create(),
                clientFactory);
        }

        public IApiCallExpression Expression(Expression<Action<TContract>> serviceCallExpr)
        {
            if(!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");
            
            var exprParams = GetParametersForExpression(mExpr);
            
            return new ApiCallExpressionWithoutReturnValue(
                _description.Url, 
                exprParams.MethodDescription,
                exprParams.Parameters,
                _httpClientFactory);
        }

        public IApiCallExpression<TRes> Expression<TRes>(Expression<Func<TContract, TRes>> serviceCallExpr)
        {
            if(!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");
            
            var exprParams = GetParametersForExpression(mExpr);
            
            return new ApiCallExpressionWithReturnValue<TRes>(
                _description.Url, 
                exprParams.MethodDescription,
                exprParams.Parameters,
                _httpClientFactory);
        }

        (MethodDescription MethodDescription, IEnumerable<ApiCallParameter> Parameters) GetParametersForExpression(MethodCallExpression serviceCallExpr)
        {
            if(!(serviceCallExpr is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");
            if(!_description.Methods.TryGetValue(mExpr.Method.MetadataToken, out var mDesc))
                throw new ApiClientException("Specified method description not found");

            var args = mExpr.Arguments;
            
            if(args.Count != mDesc.Parameters.Count)
                throw new ApiClientException("Parameters number mismatch");
            
            var callParams = new List<ApiCallParameter>();

            for (int i = 0; i < args.Count; i++)
            {
                callParams.Add(new ApiCallParameter(mDesc.Parameters[i], args[i]));
            }

            return (mDesc, callParams);
        }
    }
}