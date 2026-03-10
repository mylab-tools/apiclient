using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient.Contracts.Models;

/// <summary>
/// Represent service endpoint
/// </summary>
class EndpointModel
{
    /// <summary>
    /// Gets the relative URL for endpoint
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets the HTTP method used by the endpoint (e.g., GET, POST, PUT, DELETE).
    /// </summary>
    public HttpMethod HttpMethod { get; }

    /// <summary>
    /// Gets the list of HTTP status codes that are expected as valid responses for the endpoint.
    /// </summary>
    //todo: Вероятно вообще такое понятие исключить
    public IReadOnlyList<HttpStatusCode>? ExpectedStatusCodes { get; set; }
        
    /// <summary>
    /// Gets endpoint request parameters
    /// </summary>
    public IReadOnlyCollection<IRequestParameterModel> Parameters { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="EndpointModel"/>
    /// </summary>
    public EndpointModel(HttpMethod httpMethod, IEnumerable<IRequestParameterModel> parameters)
    {
        HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
        if(parameters == null) throw new ArgumentNullException(nameof(parameters));

        Parameters = parameters.ToArray();
    }

    /// <summary>
    /// FromMethod <see cref="EndpointModel"/> from reflection method
    /// </summary>
    public static EndpointModel FromMethod(MethodInfo mi, RequestFactoringSettings? settings = null)
    {
        if (mi == null) throw new ArgumentNullException(nameof(mi));

        var httpMethodAttr = mi.GetCustomAttribute<ApiMethodAttribute>();
        if (httpMethodAttr == null)
            throw new InvalidApiContractException($"The method '{mi.Name}' must be marked with one of ApiMethodAttribute inheritors.");

        var parameters = RequestParameterModelExtractor.FromMethod(mi, settings);

        return new EndpointModel(httpMethodAttr.HttpMethod, parameters.ToArray())
        {
            Url = httpMethodAttr.Url,
            ExpectedStatusCodes = GetExpectedStatusCodes(mi)
        };
    }

    static IReadOnlyList<HttpStatusCode> GetExpectedStatusCodes(MethodInfo mi)
    {
        return mi.GetCustomAttributes<ExpectedResponseAttribute>()
            .Select(a => a.ExpectedCode)
            .ToList();
    }
}