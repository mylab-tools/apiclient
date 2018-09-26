using System.Threading.Tasks;

namespace MyLab.ApiClient.UnitTests
{
    [Api("/foo")]
    interface ITestApiContract
    {
        [ApiPost(RelPath = "/bar")]
        Task Post(
            [QueryParam(Name = "baz")] string parameter
        );
    }
}