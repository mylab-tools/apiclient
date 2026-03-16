using System;
using System.Net;
using MyLab.ApiClient.JsonSerialization;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Usage.Invocation;

namespace MyLab.ApiClient.Tests.Usage.Invocation;

public partial class ApiClientInvocationResultBehavior
{
    readonly CallDetailsFactory _callDetailsFactory = new(
        SupportedContentDeserializers.Create
        (
            new JsonDeserializationTools(new NewtonJsonSerializer(new ApiJsonSettings())),
            new XmlDeserializationTools()
        ),
        null
    );

    public static object[][] GetCodeRangeCases()
    {
        return new[]
        {
            new object[]
            {
                HttpStatusCode.Continue,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When1xx()),
                true
            },
            new object[]
            {
                HttpStatusCode.Continue,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When2xx()),
                false
            },
            new object[]
            {
                HttpStatusCode.OK,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When2xx()),
                true
            },
            new object[]
            {
                HttpStatusCode.OK,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When1xx()),
                false
            },
            new object[]
            {
                HttpStatusCode.Redirect,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When3xx()),
                true
            },
            new object[]
            {
                HttpStatusCode.Redirect,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When1xx()),
                false
            },
            new object[]
            {
                HttpStatusCode.NotFound,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When4xx()),
                true
            },
            new object[]
            {
                HttpStatusCode.NotFound,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When1xx()),
                false
            },
            new object[]
            {
                HttpStatusCode.ServiceUnavailable,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When5xx()),
                true
            },
            new object[]
            {
                HttpStatusCode.ServiceUnavailable,
                (Func<ApiClientInvocationResult, ProcessingActionBuilder>)(r => r.When1xx()),
                false
            },
        };
    }
}