using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.JsonSerialization;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Newtonsoft.Json;
using Xunit;

namespace MyLab.ApiClient.Tests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(StructuredObjectContentDeserializer))]
public class StructuredObjectContentDeserializerBehavior
{
    readonly StructuredObjectContentDeserializer _deserializer = new
    (
        new JsonDeserializationTools(NewtonJsonSerializer.Default),
        new XmlDeserializationTools()
    );

    [Fact]
    public async Task ShouldDeserializeValidStructuredObjectFromJsonContent()
    {
        // Arrange
        var jsonObject = "{\"Id\": 1, \"Name\": \"Test\"}";
        var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(TestObject));
        // Assert
        var deserializedObject = Assert.IsType<TestObject>(result);
        Assert.Equal(1, deserializedObject.Id);
        Assert.Equal("Test", deserializedObject.Name);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidStructuredObjectContent()
    {
        // Arrange
        var invalidJson = "\"invalid\"";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<JsonReaderException>(() =>
            _deserializer.DeserializeAsync(content, typeof(TestObject)));
    }

    class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}