using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class HttpRequestMessageBuilderBehavior
    {
        [Theory]
        [MemberData(nameof(GetUrlCalcCases))]
        public void ShouldCalculateRightUrl(Type contractType, string methodName, string baseAddress, string expectedUrl)
        {
            //Arrange
            var srvDesc = ServiceDescription.Create(contractType);

            var method = contractType.GetMethod(methodName);

            var mDesc = srvDesc.Methods
                .Where(kv => kv.Key == method.MetadataToken)
                .Select(kv => kv.Value)
                .First();

            var reqMsgBuilder = new HttpRequestMessageBuilder(baseAddress, mDesc);

            //Act
            var reqMsg = reqMsgBuilder.Build();
            
            //Assert
            Assert.Equal(expectedUrl, reqMsg.RequestUri.OriginalString);
        }

        public static IEnumerable<object[]> GetUrlCalcCases()
        {
            var baseAddresses = new []
            {
                "/srv/",
                "http://localhost/",
                "/"
            };
            var list = new List<object[]>();

            foreach (var baseAddress in baseAddresses)
            {
                list.Add(new object[] {typeof(IContract1), nameof(IContract1.Post), baseAddress, baseAddress + "foo" });
                list.Add(new object[] {typeof(IContract2), nameof(IContract2.Post), baseAddress, baseAddress.TrimEnd('/') });
                list.Add(new object[] {typeof(IContract3), nameof(IContract3.Post), baseAddress, baseAddress + "bar" });
            }

            return list;
        }

        [Api("")]
        public interface IContract1
        {
            [Post("foo")]
            Task<string> Post([JsonContent] string request);
        }

        [Api("foo")]
        public interface IContract2
        {
            [Post("")]
            Task<string> Post([JsonContent] string request);
        }

        [Api("foo")]
        public interface IContract3
        {
            [Post("bar")]
            Task<string> Post([JsonContent] string request);
        }
    }
}
