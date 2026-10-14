using System;
using System.Net;

namespace MyLab.ApiClient;

/// <summary>
/// Legacy expected HTTP status attribute from MyLab.ApiClient 3.x.
/// </summary>
[Obsolete("Legacy MyLab.ApiClient 3.x attribute retained for type unification.")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ExpectedCodeAttribute : Attribute
{
    /// <summary>
    /// Gets the expected HTTP status code.
    /// </summary>
    public HttpStatusCode ExpectedCode { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ExpectedCodeAttribute"/>.
    /// </summary>
    public ExpectedCodeAttribute(HttpStatusCode expectedCode)
    {
        ExpectedCode = expectedCode;
    }
}
