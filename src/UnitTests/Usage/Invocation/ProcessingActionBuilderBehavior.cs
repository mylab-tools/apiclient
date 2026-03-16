using System.Collections.Generic;
using JetBrains.Annotations;
using MyLab.ApiClient.JsonSerialization;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Problems;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Usage.Invocation;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyLab.ApiClient.Tests.Usage.Invocation;

[TestSubject(typeof(ProcessingActionBuilder))]
public class ProcessingActionBuilderBehavior
{
    readonly CallDetailsFactory _callDetailsFactory = new
    (
        SupportedContentDeserializers.Create
        (
            new JsonDeserializationTools(new NewtonJsonSerializer(new ApiJsonSettings())),
            new XmlDeserializationTools()
        ),
        null
    );
    
    [Fact]
    public async Task ShouldAddProcessResultActionIntoContext()
    {
        //Arrange
        var callDetail = await _callDetailsFactory.CreateAsync
        (
            new HttpRequestMessage(),
            TestTools.CreateOkResponse("foo")
        );
        var invocationResult = new ApiClientInvocationResult(callDetail, new CallResultProcessingContext());

        var builder = new ProcessingActionBuilder(invocationResult, d => d.StatusCode == HttpStatusCode.OK);

        string? capturedString = null;
        HttpStatusCode capturedStatusCode = default;
        
        //Act
        var newInvocationResult = builder.ProcessResult<string>((s, c) =>
        {
            capturedString = s;
            capturedStatusCode = c;
        });

        var ctxActions = newInvocationResult.ProcessingContext.ProcessingActions;
        var action = ctxActions?.FirstOrDefault();

        if (action != null)
        {
            await action.PerformAsync(callDetail, CancellationToken.None);
        }
        
        //Assert
        Assert.NotNull(ctxActions);
        Assert.Single(ctxActions);
        Assert.NotNull(action);
        Assert.Equal("foo", capturedString);
        Assert.Equal(HttpStatusCode.OK, capturedStatusCode);
    }

    [Fact]
    public async Task ShouldAddProcessProblemActionIntoContext()
    {
        //Arrange
        var problem = new ProblemDetails
        {
            Title = "foo"
        };
        var problemResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent
                (
                    JsonConvert.SerializeObject(problem),
                    Encoding.UTF8,
                    "application/problem+json"
                )
        };
        
        var callDetail = await _callDetailsFactory.CreateAsync
        (
            new HttpRequestMessage(),
            problemResponse
        );
        var invocationResult = new ApiClientInvocationResult(callDetail, new CallResultProcessingContext());

        var builder = new ProcessingActionBuilder(invocationResult, d => d.StatusCode == HttpStatusCode.NotFound);

        ProblemDetails? capturedProblem = null;
        HttpStatusCode capturedStatusCode = default;

        //Act
        var newInvocationResult = builder.ProcessProblem((p, c) =>
        {
            capturedProblem = p;
            capturedStatusCode = c;
        });

        var ctxActions = newInvocationResult.ProcessingContext.ProcessingActions;
        var action = ctxActions?.FirstOrDefault();

        if (action != null)
        {
            await action.PerformAsync(callDetail, CancellationToken.None);
        }

        //Assert
        Assert.NotNull(ctxActions);
        Assert.Single(ctxActions);
        Assert.NotNull(action);
        Assert.NotNull(capturedProblem);
        Assert.Equal("foo", capturedProblem.Title);
        Assert.Equal(HttpStatusCode.NotFound, capturedStatusCode);
    }

    [Fact]
    public async Task ShouldAddProcessValidationProblemActionIntoContext()
    {
        //Arrange
        var problem = new ValidationProblemDetails
        {
            Title = "foo",
            Errors = new Dictionary<string, string[]>
            {
                {"foo", ["bar"] }
            }
        };
        var problemResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent
            (
                JsonConvert.SerializeObject(problem),
                Encoding.UTF8,
                "application/problem+json"
            )
        };

        var callDetail = await _callDetailsFactory.CreateAsync
        (
            new HttpRequestMessage(),
            problemResponse
        );
        var invocationResult = new ApiClientInvocationResult(callDetail, new CallResultProcessingContext());

        var builder = new ProcessingActionBuilder(invocationResult, d => d.StatusCode == HttpStatusCode.NotFound);

        
        ValidationProblemDetails? capturedProblem = null;
        HttpStatusCode capturedStatusCode = default;

        //Act
        var newInvocationResult = builder.ProcessValidationProblem((p, c) =>
        {
            capturedProblem = p;
            capturedStatusCode = c;
        });

        var ctxActions = newInvocationResult.ProcessingContext.ProcessingActions;
        var action = ctxActions?.FirstOrDefault();

        if (action != null)
        {
            await action.PerformAsync(callDetail, CancellationToken.None);
        }

        //Assert
        Assert.NotNull(ctxActions);
        Assert.Single(ctxActions);
        Assert.NotNull(action);
        Assert.NotNull(capturedProblem);
        Assert.Equal("foo", capturedProblem.Title);
        Assert.NotNull(capturedProblem.Errors);
        Assert.Single(capturedProblem.Errors);
        Assert.True(capturedProblem.Errors.ContainsKey("foo"));
        Assert.NotNull(capturedProblem.Errors["foo"]);
        Assert.Single(capturedProblem.Errors["foo"]);
        Assert.Equal("bar", capturedProblem.Errors["foo"][0]);
        Assert.Equal(HttpStatusCode.NotFound, capturedStatusCode);
    }
}