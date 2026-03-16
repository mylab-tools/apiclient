using JetBrains.Annotations;
using Moq;
using MyLab.ApiClient.ResponseProcessing;
using MyLab.ApiClient.Usage.Invocation;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyLab.ApiClient.Tests.Usage.Invocation;

[TestSubject(typeof(ApiClientInvocationResult))]
public partial class ApiClientInvocationResultBehavior
{
    [Fact]
    public async Task ShouldAddHttpCodeIntoContext()
    {
        //Arrange
        var req = TestTools.CreateSimpleRequest();
        var successResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        var callDetails = await _callDetailsFactory.CreateAsync(req, successResponse);

        var invocationResult = new ApiClientInvocationResult(callDetails, new CallResultProcessingContext());

        //Act
        var newInvocation = invocationResult.When(HttpStatusCode.NotFound)
            .ProcessProblem((_,_) => {});

        var ctx = newInvocation.ProcessingContext;
        
        //Assert
        Assert.NotNull(ctx.HandledStatusPredicates);
        Assert.Single(ctx.HandledStatusPredicates);
        Assert.Contains(ctx.HandledStatusPredicates, predicate => predicate(HttpStatusCode.NotFound));

        Assert.NotNull(ctx.ProcessingActions);
        Assert.Single(ctx.ProcessingActions);
        Assert.Contains(ctx.ProcessingActions, act => act.Predicate(newInvocation.CallDetails));
    }

    [Fact]
    public async Task ShouldAddAdditionalHttpCodeIntoContext()
    {
        //Arrange
        var req = TestTools.CreateSimpleRequest();
        var successResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        var callDetails = await _callDetailsFactory.CreateAsync(req, successResponse);

        var invocationResult = new ApiClientInvocationResult(callDetails, new CallResultProcessingContext());

        //Act
        var newInvocation = invocationResult.When(HttpStatusCode.OK,HttpStatusCode.NotFound)
            .ProcessProblem((_, _) => { });

        var ctx = newInvocation.ProcessingContext;

        //Assert
        Assert.NotNull(ctx.HandledStatusPredicates);
        Assert.Equal(2, ctx.HandledStatusPredicates.Count);
        Assert.Contains(ctx.HandledStatusPredicates, predicate => predicate(HttpStatusCode.NotFound));
        Assert.Contains(ctx.HandledStatusPredicates, predicate => predicate(HttpStatusCode.OK));

        Assert.NotNull(ctx.ProcessingActions);
        Assert.Single(ctx.ProcessingActions);
        Assert.Contains(ctx.ProcessingActions, act => act.Predicate(newInvocation.CallDetails));
    }

    [Theory]
    [MemberData(nameof(GetCodeRangeCases))]
    public async Task ShouldAddHttpCodeRangeIntoContext
        (
            HttpStatusCode actualCode,
            Func<ApiClientInvocationResult, ProcessingActionBuilder> methodSelector,
            bool successfulResultExpected
        )
    {
        //Arrange
        var req = TestTools.CreateSimpleRequest();
        var successResponse = new HttpResponseMessage(actualCode);
        var callDetails = await _callDetailsFactory.CreateAsync(req, successResponse);

        var invocationResult = new ApiClientInvocationResult(callDetails, new CallResultProcessingContext());

        //Act
        var newInvocation = methodSelector(invocationResult)
            .ProcessProblem((_, _) => { });

        var ctx = newInvocation.ProcessingContext;

        //Assert

        Assert.NotNull(ctx.HandledStatusPredicates);
        Assert.Single(ctx.HandledStatusPredicates);
        Assert.NotNull(ctx.ProcessingActions);
        Assert.Single(ctx.ProcessingActions);
        
        if (successfulResultExpected)
        {
            Assert.Contains(ctx.HandledStatusPredicates, predicate => predicate(actualCode));
            Assert.Contains(ctx.ProcessingActions, act => act.Predicate(newInvocation.CallDetails));
        }
        else
        {
            Assert.DoesNotContain(ctx.HandledStatusPredicates, predicate => predicate(actualCode));
            Assert.DoesNotContain(ctx.ProcessingActions, act => act.Predicate(newInvocation.CallDetails));
        }
    }

    [Fact]
    public async Task ShouldSetThrowIfAnotherStatusCodeFlagIntoContext()
    {
        //Arrange
        var req = TestTools.CreateSimpleRequest();
        var successResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        var callDetails = await _callDetailsFactory.CreateAsync(req, successResponse);

        var invocationResult = new ApiClientInvocationResult(callDetails, new CallResultProcessingContext());

        //Act
        var newInvocation = invocationResult.ThrowIfAnotherStatusCode();

        //Assert
        Assert.False(invocationResult.ProcessingContext.ThrowIfAnotherStatusCode);
        Assert.True(newInvocation.ProcessingContext.ThrowIfAnotherStatusCode);
    }

    [Fact]
    public async Task ShouldPerformProcessing()
    {
        //Arrange
        var req = TestTools.CreateSimpleRequest();
        var successResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        var callDetails = await _callDetailsFactory.CreateAsync(req, successResponse);

        var procStrategy = new Mock<IFluentProcessingStrategy>();
        
        var invocationResult = new ApiClientInvocationResult(callDetails, new CallResultProcessingContext())
        {
            ProcessingStrategy = procStrategy.Object
        };

        //Act
        var newResult = invocationResult
            .When2xx()
            .ProcessResult<string>((_, _) => { });

        await newResult.ProcessAsync(CancellationToken.None);

        //Assert
        procStrategy.Verify(s => s.ProcessAsync
            (
                It.Is<CallDetails>(d => d == invocationResult.CallDetails),
                It.Is<CallResultProcessingContext>(c => c == newResult.ProcessingContext),
                It.Is<CancellationToken>(t => t == CancellationToken.None)
            ));
    }
}