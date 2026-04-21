using JetBrains.Annotations;
using MyLab.ApiClient.Usage.Invocation2;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Tests;
using Xunit;

namespace UnitTests.Usage
{
    [TestSubject(typeof(ApiClientInvoker<>))]
    public class ApiClientInvokerBehavior
    {
        [Fact]
        public async Task ShouldSendRequest()
        {
            //Arrange
            var reqProc = TestTools.CreateReqProcMock
                (
                    TestTools.CreateOkResponse()
                );

            var invoker = ApiClientInvoker<IContract>.FromContract(reqProc.Object, TestTools.DefaultOptions);

            //Act
            var callDetails = await invoker
                .ForExpression(c => c.GetAsync())
                .InvokeAsync(TestContext.Current.CancellationToken);
            
            //Assert
            TestTools.VerifyRequestUrl(reqProc, "get");

            Assert.NotNull(callDetails);
            Assert.Equal(HttpStatusCode.OK, callDetails.StatusCode);
            Assert.Equal(HttpMethod.Get, callDetails.RequestMessage.Method);
        }

        [Fact]
        public async Task ShouldTransferContent()
        {
            //Arrange
            var reqProc = TestTools.CreateReqProcMock
            (
                TestTools.CreateOkResponse("foo")
            );

            var invoker = ApiClientInvoker<IContract>.FromContract(reqProc.Object, TestTools.DefaultOptions);

            //Act

            var callDetails = await invoker
                .ForExpression(c => c.EchoAsync("foo"))
                .InvokeAsync(TestContext.Current.CancellationToken);

            var result = await callDetails.ReadContentAsync<string>();
            
            //Assert
            TestTools.VerifyRequestUrl(reqProc, "echo");
            TestTools.VerifyRequestContent(reqProc, "foo");

            Assert.NotNull(callDetails);
            Assert.Equal(HttpStatusCode.OK, callDetails.StatusCode);
            Assert.Equal(HttpMethod.Post, callDetails.RequestMessage.Method);
            Assert.Equal("foo", result);
        }

        [Theory]
        [MemberData(nameof(GetHandlersCases))]
        public async Task ShouldSupportFluentHandlers(
            HttpResponseMessage response, 
            string expectedResultContent,
            HttpStatusCode expectedStatusCode
            )
        {
            //Arrange
            var reqProc = TestTools.CreateReqProcMock(response);

            var invoker = ApiClientInvoker<IContract>.FromContract(reqProc.Object, TestTools.DefaultOptions);

            string? actualResult = null;
            HttpStatusCode actualStatusCode = default;

            //Act

            var callDetails = await invoker
                .ForExpression(c => c.EchoAsync("foo"))
                .When2xx().ProcessResult<string>((res, code) =>
                {
                    actualResult = res;
                    actualStatusCode = code;
                })
                .When4xx().ProcessStatusCode(code =>
                {
                    actualStatusCode = code;
                })
                .InvokeAsync(TestContext.Current.CancellationToken);

            //Assert
            TestTools.VerifyRequestUrl(reqProc, "echo");
            TestTools.VerifyRequestContent(reqProc, "foo");

            Assert.NotNull(callDetails);
            Assert.Equal(expectedStatusCode, callDetails.StatusCode);
            Assert.Equal(HttpMethod.Post, callDetails.RequestMessage.Method);
            Assert.Equal(expectedResultContent, actualResult);
            Assert.Equal(expectedStatusCode, actualStatusCode);
        }

        [Theory]
        [MemberData(nameof(GetHandlersCases))]
        public async Task ShouldSupportFluentAsyncHandlers(
            HttpResponseMessage response,
            string expectedResultContent,
            HttpStatusCode expectedStatusCode
        )
        {
            //Arrange
            var reqProc = TestTools.CreateReqProcMock(response);

            var invoker = ApiClientInvoker<IContract>.FromContract(reqProc.Object, TestTools.DefaultOptions);

            string? actualResult = null;
            HttpStatusCode actualStatusCode = default;

            //Act

            var callDetails = await invoker
                .ForExpression(c => c.EchoAsync("foo"))
                .When2xx().ProcessResultAsync<string>((res, code) =>
                {
                    actualResult = res;
                    actualStatusCode = code;

                    return Task.CompletedTask;
                })
                .When4xx().ProcessStatusCodeAsync(code =>
                {
                    actualStatusCode = code;

                    return Task.CompletedTask;
                })
                .InvokeAsync(TestContext.Current.CancellationToken);

            //Assert
            TestTools.VerifyRequestUrl(reqProc, "echo");
            TestTools.VerifyRequestContent(reqProc, "foo");

            Assert.NotNull(callDetails);
            Assert.Equal(expectedStatusCode, callDetails.StatusCode);
            Assert.Equal(HttpMethod.Post, callDetails.RequestMessage.Method);
            Assert.Equal(expectedResultContent, actualResult);
            Assert.Equal(expectedStatusCode, actualStatusCode);
        }

        public static object?[][] GetHandlersCases()
        {
            var successStringResponse = new HttpResponseMessage(HttpStatusCode.OK);
            successStringResponse.Content = new StringContent("foo");

            return new object?[][]
            {
                [
                    successStringResponse,
                    "foo",
                    HttpStatusCode.OK
                ],
                [
                    new HttpResponseMessage(HttpStatusCode.NotFound),
                    null,
                    HttpStatusCode.NotFound
                ]
            };
        }

        interface IContract
        {
            [Get("get")]
            Task GetAsync();

            [Post("echo")]
            Task<string> EchoAsync([StringContent] string value);
        }
    }
}
