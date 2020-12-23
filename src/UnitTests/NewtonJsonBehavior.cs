using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace UnitTests
{
    public class NewtonJsonBehavior
    {
        [Fact]
        public void ShouldSerializeEnumAsLiteral()
        {
            //Arrange
            

            //Act
            var str = JsonConvert.SerializeObject(TestEnum.Value);

            //Assert
            Assert.Equal("\"val\"", str);
        }

        [Fact]
        public void ShouldDeserializeEnumFromLiteral()
        {
            //Arrange


            //Act
            var enm = JsonConvert.DeserializeObject<TestEnum>("\"val\"");

            //Assert
            Assert.Equal(TestEnum.Value, enm);
        }

        [Fact]
        public void ShouldSerializerDeserializeEnumFromLiteral()
        {
            //Arrange


            //Act
            var d = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            TestEnum res;

            using (var r = new StringReader("\"val\""))
            {
                res = (TestEnum)d.Deserialize(r, typeof(TestEnum));
            }

            //Assert
            Assert.Equal(TestEnum.Value, res);
        }

        [Fact]
        public void ShouldNotSerializerDeserializeEnumFromLiteralWithoutQuotes()
        {
            //Arrange


            //Act
            var d = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            TestEnum res;

            Assert.Throws<JsonReaderException>(() =>
            {
                using (var r = new StringReader("val"))
                {
                    res = (TestEnum)d.Deserialize(r, typeof(TestEnum));
                }
            });
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum TestEnum
        {
            Undefined,
            [EnumMember(Value = "val")]
            Value
        }
    }

}
