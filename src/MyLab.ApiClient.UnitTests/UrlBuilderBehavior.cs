using Xunit;

namespace MyLab.ApiClient.UnitTests
{
    public class UrlBuilderBehavior
    {
        [Fact]
        public void ShouldCreateUrlWithContractRelPath()
        {
            //Arrange
            var urlBuilder = GetBuilder<IContract>(nameof(IContract.WithoutPath));

            //Act
            var url = urlBuilder.Build(new object[0]);

            //Assert
            Assert.Equal("foo", url);
        }

        [Fact]
        public void ShouldCreateUrlWithMethodRelPath()
        {
            //Arrange
            var urlBuilder = GetBuilder<IContractWithoutRelPath>(nameof(IContractWithoutRelPath.WithPath));

            //Act
            var url = urlBuilder.Build(new object[0]);

            //Assert
            Assert.Equal("foo", url);
        }

        [Fact]
        public void ShouldCreateUrlWithCombinedRelPaths()
        {
            //Arrange
            var urlBuilder = GetBuilder<IContract>(nameof(IContract.WithPath));

            //Act
            var url = urlBuilder.Build(new object[0]);

            //Assert
            Assert.Equal("foo/bar", url);
        }

        [Fact]
        public void ShouldCreateUrlWithPathArg()
        {
            //Arrange
            var urlBuilder = GetBuilder<IContract>(nameof(IContract.WithPathArg));

            //Act
            var url = urlBuilder.Build(new object[]{ 10 });

            //Assert
            Assert.Equal("foo/bar/10", url);
        }

        [Fact]
        public void ShouldCreateUrlWithQueryArg()
        {
            //Arrange
            var urlBuilder = GetBuilder<IContract>(nameof(IContract.WithQueryArg));

            //Act
            var url = urlBuilder.Build(new object[] { 10 });

            //Assert
            Assert.Equal("foo/bar?index=10", url);
        }

        UrlBuilder GetBuilder<TContract>(string methodName)
        {
            var contract = typeof(TContract);
            var desc = ApiClientDescription.Get(contract);

            var methodToken = contract.GetMethod(methodName).MetadataToken;
            return UrlBuilder.GetForMethod(desc, methodToken);
        }

        [Api("foo")]
        interface IContract
        {
            [ApiPost]
            void WithoutPath();

            [ApiPost(RelPath = "bar")]
            void WithPath();

            [ApiPost(RelPath = "bar/{index}")]
            void WithPathArg([ApiParam(ApiParamPlace.Path)]int index);

            [ApiPost(RelPath = "bar")]
            void WithQueryArg([ApiParam(ApiParamPlace.Query)]int index);
        }

        [Api]
        interface IContractWithoutRelPath
        {
            [ApiPost(RelPath = "foo")]
            void WithPath();
        }
    }
}
