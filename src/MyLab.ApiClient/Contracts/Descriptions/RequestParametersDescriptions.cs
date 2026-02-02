using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;

namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes API request method parameters
/// </summary>
class RequestParametersDescriptions
{
    /// <summary>
    /// Gets parameters which will modify a URL  
    /// </summary>
    public IReadOnlyList<UrlParameterDescription>? UrlParams { get; set; }
    /// <summary>
    /// Gets parameters which will be added into headers
    /// </summary>
    public IReadOnlyList<HeaderParameterDescription>? HeaderParams { get; set; }
    /// <summary>
    /// Gets parameters which will be used to create request content
    /// </summary>
    public IReadOnlyList<ContentParameterDescription>? ContentParams { get; set; }
    /// <summary>
    /// Gets parameters which is a collection of the headers, and it will be added into headers
    /// </summary>
    public IReadOnlyList<HeaderCollectionParameterDescription>? HeaderCollectionParams { get; set; }
        
    /// <summary>
    /// Creates <see cref="RequestParametersDescriptions"/> from reflection <see cref="MethodInfo"/>
    /// </summary>
    public static RequestParametersDescriptions FromMethod(MethodInfo mi)
    {
        if (mi == null) throw new ArgumentNullException(nameof(mi));
            
        var urlParams = new List<UrlParameterDescription>();
        var headerParams = new List<HeaderParameterDescription>();
        var headerCollectionParams = new List<HeaderCollectionParameterDescription>();
        var contentParams = new List<ContentParameterDescription>();

        var props = mi.GetParameters();

        for (int i = 0; i < props.Length; i++)
        {
            var p = props[i];

            var attr = GetParamAttr(p);

            switch (attr)
            {
                case UrlParameterAttribute urlA:
                    urlParams.Add(new UrlParameterDescription(i, urlA.Name ?? p.Name!, urlA.UrlModifier));
                    break;
                case HeaderAttribute hdrA:
                    headerParams.Add(new HeaderParameterDescription(i, hdrA.Name ?? p.Name!));
                    break;
                case HeaderCollectionAttribute:
                    headerCollectionParams.Add(new HeaderCollectionParameterDescription(i));
                    break;
                case ContentParameterAttribute contA:
                    contentParams.Add(new ContentParameterDescription(i, contA.HttpContentFactory));
                    break;
                default:
                    throw new InvalidApiContractException($"Request attribute type '{attr.GetType().FullName}' is not supported");
            }
        }

        return new RequestParametersDescriptions
        {
            UrlParams = urlParams,
            HeaderParams = headerParams,
            ContentParams = contentParams,
            HeaderCollectionParams = headerCollectionParams
        };
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