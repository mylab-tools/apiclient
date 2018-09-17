namespace MyLab.ApiClient.UnitTests
{
    [Api("/foo")]
    interface ITestApiContract
    {
        [ApiPost(RelPath = "/bar")]
        void Post(
            [ApiParam(ApiParamPlace.Query, Name = "baz")] string parameter
        );
    }
}