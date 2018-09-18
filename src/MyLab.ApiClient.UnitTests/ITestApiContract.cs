using System.Threading.Tasks;

namespace MyLab.ApiClient.UnitTests
{
    [Api("/foo")]
    interface ITestApiContract
    {
        [ApiPost(RelPath = "/bar")]
        Task Post(
            [ApiParam(ApiParamPlace.Query, Name = "baz")] string parameter
        );
    }
}