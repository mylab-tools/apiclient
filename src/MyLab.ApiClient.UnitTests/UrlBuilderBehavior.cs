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

        [Fact]
        public void ShouldCreateUrlWithQueryAndPath()
        {
            //Arrange
            var contract = typeof(IContract);
            var desc = ApiClientDescription.Get(contract);

            var methodToken = contract.GetMethod(nameof(IContract.WithQueryAndPathArg)).MetadataToken;
            var urlBuilder = UrlBuilder.GetForMethod(desc, methodToken);

            //Act
            var url = urlBuilder.Build(new object[] { 2881, 2 });

            //Assert
            Assert.Equal("foo/v1/docs/2881/signature-validation-result?mode=2", url);
        }

        [Api("foo")]
        interface IContract
        {
            [ApiPost]
            Task WithoutPath();

            [ApiPost(RelPath = "bar")]
            Task WithPath();

            [ApiPost(RelPath = "bar/{index}")]
            Task WithPathArg([PathParam]int index);

            [ApiPost(RelPath = "bar")]
            Task WithQueryArg([QueryParam]int index);

            [ApiPost(RelPath = "v1/docs/{id}/signature-validation-result")]
            Task WithQueryAndPathArg([PathParam] int id, [QueryParam]int mode);
        }

        [Api]
        interface IContractWithoutRelPath
        {
            [ApiPost(RelPath = "foo")]
            Task WithPath();
        }
    }
}
