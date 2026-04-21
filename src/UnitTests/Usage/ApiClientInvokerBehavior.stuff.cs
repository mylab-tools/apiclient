using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;

namespace UnitTests.Usage;

public partial class ApiClientInvokerBehavior
{
    public static object?[][] GetUnexpectedTestCases()
    {
        return new object?[][]
        {
            [
                new HttpResponseMessage(HttpStatusCode.BadRequest),
                true
            ],
            [
                new HttpResponseMessage(HttpStatusCode.NotFound),
                false
            ]
        };
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