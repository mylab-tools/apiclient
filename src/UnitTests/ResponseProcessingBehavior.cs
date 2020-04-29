using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ResponseProcessingBehavior
    {
        [Theory]
        [InlineData(typeof(bool), "true", true)]
        [InlineData(typeof(bool), "True", true)]
        [InlineData(typeof(bool), "TRUE", true)]
        [InlineData(typeof(bool), "false", false)]
        [InlineData(typeof(bool), "False", false)]
        [InlineData(typeof(bool), "FALSE", false)]
        [InlineData(typeof(int), "1", 1)]
        [InlineData(typeof(int), "-1", -1)]
        [InlineData(typeof(uint), "1", 1u)]
        [InlineData(typeof(uint), "0", 0u)]
        [InlineData(typeof(double), "1.0", 1.0)]
        [InlineData(typeof(double), "-1.0", -1.0)]
        [InlineData(typeof(double), "1,0", 1.0)]
        [InlineData(typeof(double), "-1,0", -1.0)]
        [InlineData(typeof(string), "foo", "foo")]
        [MemberData(nameof(GetTimeSpanForDeserialization))]
        [MemberData(nameof(GetDateTimeForDeserialization))]
        [MemberData(nameof(GetGuidForDeserialization))]
        public async Task ShouldDeserializeSimpleContent(Type targetType, string strContent, object expected)
        {
            //Arrange
            var httpContent = new StringContent(strContent);

            //Act
            var res = await ResponseProcessing.DeserializeContent(targetType, httpContent);

            //Assert
            Assert.Equal(expected, res);
        }

        [Theory]
        [InlineData("<Root><TestValue>foo</TestValue></Root>", "application/xml")]
        [InlineData("{\"TestValue\":\"foo\"}", "application/json")]
        [InlineData("{TestValue:\"foo\"}", "application/json")]
        public async Task ShouldDeserializeContent(string strContent, string mediaType)
        {
            //Arrange
            var httpContent = new StringContent(strContent, Encoding.UTF8, mediaType);

            //Act
            var res = await ResponseProcessing.DeserializeContent<TestModel>(httpContent);

            //Assert
            Assert.Equal("foo", res.TestValue);
        }

        public class TestModel
        {
            public string TestValue { get; set; }
        }

        public static IEnumerable<object[]> GetTimeSpanForDeserialization()
        {
            yield return new object[]
            {
                typeof(TimeSpan), 
                "1", 
                TimeSpan.FromDays(1)
            };
            yield return new object[]
            {
                typeof(TimeSpan), 
                "1:01", 
                TimeSpan
                    .FromHours(1)
                    .Add(TimeSpan.FromMinutes(1))
            };
            yield return new object[]
            {
                typeof(TimeSpan),
                "1:01:02",
                TimeSpan
                    .FromHours(1)
                    .Add(TimeSpan.FromMinutes(1))
                    .Add(TimeSpan.FromSeconds(2))
            };
            yield return new object[]
            {
                typeof(TimeSpan),
                "1:01:02.006",
                TimeSpan
                    .FromHours(1)
                    .Add(TimeSpan.FromMinutes(1))
                    .Add(TimeSpan.FromSeconds(2))
                    .Add(TimeSpan.FromMilliseconds(6))
            };
            yield return new object[]
            {
                typeof(TimeSpan),
                "3.1:01:02.006",
                TimeSpan
                    .FromDays(3)
                    .Add(TimeSpan.FromHours(1))
                    .Add(TimeSpan.FromMinutes(1))
                    .Add(TimeSpan.FromSeconds(2))
                    .Add(TimeSpan.FromMilliseconds(6))
            };
        }

        public static IEnumerable<object[]> GetDateTimeForDeserialization()
        {
            yield return new object[]
            {
                typeof(DateTime),
                "08/18/2018 07:22:16",
                new DateTime(2018, 08,18 ,7, 22, 16) 
            };
            yield return new object[]
            {
                typeof(DateTime),
                "08/18/2018",
                new DateTime(2018, 08,18 ,0, 0, 0)
            };
            yield return new object[]
            {
                typeof(DateTime),
                "2018-08-18T07:22:16.0000000Z",
                new DateTime(2018, 08,18 ,10, 22, 16)
            };
            yield return new object[]
            {
                typeof(DateTime),
                "2018-08-18T07:22:16.0000000-07:00",
                new DateTime(2018, 08,18 ,17, 22, 16)
            };
            yield return new object[]
            {
                typeof(DateTime),
                "Sat, 18 Aug 2018 07:22:16 GMT",
                new DateTime(2018, 08,18 ,10, 22, 16)
            };
            yield return new object[]
            {
                typeof(DateTime),
                "08/18/2018 07:22:16 -5:00",
                new DateTime(2018, 08,18 ,15, 22, 16)
            };
        }

        public static IEnumerable<object[]> GetGuidForDeserialization()
        {
            yield return new object[]
            {
                typeof(Guid),
                "76e95f31-e9ee-48aa-9953-0dc4871ccbda",
                new Guid("76e95f31-e9ee-48aa-9953-0dc4871ccbda")
            };
            yield return new object[]
            {
                typeof(Guid),
                "{76e95f31-e9ee-48aa-9953-0dc4871ccbda}",
                new Guid("{76e95f31-e9ee-48aa-9953-0dc4871ccbda}"), 
            };
            yield return new object[]
            {
                typeof(Guid),
                "{FCE3D0C1-FEB9-46B3-875B-1723F286D09A}",
                new Guid("{FCE3D0C1-FEB9-46B3-875B-1723F286D09A}"), 
            };
        }
    }
}
