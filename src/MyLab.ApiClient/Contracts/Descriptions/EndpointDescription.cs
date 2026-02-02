using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;

namespace MyLab.ApiClient.Contracts.Descriptions;

/// <summary>
/// Describes service endpoint
/// </summary>
class EndpointDescription
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
    public IReadOnlyList<HttpStatusCode>? ExpectedStatusCodes { get; set; }
        
    /// <summary>
    /// Gets endpoint request parameters
    /// </summary>
    public RequestParametersDescriptions Parameters { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="EndpointDescription"/>
    /// </summary>
    public EndpointDescription(HttpMethod httpMethod, RequestParametersDescriptions parameters)
    {
        HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
        Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    /// <summary>
    /// FromMethod <see cref="EndpointDescription"/> from reflection method
    /// </summary>
    public static EndpointDescription FromMethod(MethodInfo mi)
    {
        if (mi == null) throw new ArgumentNullException(nameof(mi));

        var httpMethodAttr = mi.GetCustomAttribute<ApiMethodAttribute>();
        if (httpMethodAttr == null)
            throw new InvalidApiContractException($"The method '{mi.Name}' must be marked with one of ApiMethodAttribute inheritors.");

        var parameters = RequestParametersDescriptions.FromMethod(mi);

        return new EndpointDescription(httpMethodAttr.HttpMethod, parameters)
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