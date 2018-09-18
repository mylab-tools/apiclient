using System;
using System.Threading.Tasks;
using Xunit;

namespace MyLab.ApiClient.UnitTests
{
    public class UrlBuilderBehavior
    {
        [Theory]
        [InlineData(typeof(IContract), nameof(IContract.WithoutPath), "foo")]
        [InlineData(typeof(IContractWithoutRelPath), nameof(IContractWithoutRelPath.WithPath), "foo")]
        [InlineData(typeof(IContract), nameof(IContract.WithPath), "foo/bar")]
        [InlineData(typeof(IContract), nameof(IContract.WithPathArg), "foo/bar/10")]
        [InlineData(typeof(IContract), nameof(IContract.WithQueryArg), "foo/bar?index=10")]
        public void ShouldCreateUrlWith(Type contract, string methodName, string expectedUrl)
        {
            //Arrange
            var desc = ApiClientDescription.Get(contract);

            var methodToken = contract.GetMethod(methodName).MetadataToken;
            var urlBuilder = UrlBuilder.GetForMethod(desc, methodToken);

            //Act
            var url = urlBuilder.Build(new object[]{10});

            //Assert
            Assert.Equal(expectedUrl, url);
        }

        [Api("foo")]
        interface IContract
        {
            [ApiPost]
            Task WithoutPath();

            [ApiPost(RelPath = "bar")]
            Task WithPath();

            [ApiPost(RelPath = "bar/{index}")]
            Task WithPathArg([ApiParam(ApiParamPlace.Path)]int index);

            [ApiPost(RelPath = "bar")]
            Task WithQueryArg([ApiParam(ApiParamPlace.Query)]int index);
        }

        [Api]
        interface IContractWithoutRelPath
        {
            [ApiPost(RelPath = "foo")]
            Task WithPath();
        }
    }
}
