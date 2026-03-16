using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.JsonSerialization;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Newtonsoft.Json;
using Xunit;

namespace MyLab.ApiClient.Tests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(EnumerableContentDeserializer))]
public class EnumerableContentDeserializerBehavior
{
    readonly EnumerableContentDeserializer _deserializer = new
        (
            new JsonDeserializationTools(NewtonJsonSerializer.Default),
            new XmlDeserializationTools()
        );
    
    [Fact]
    public async Task ShouldDeserializeValidEnumerableFromJsonContent()
    {
        // Arrange
        var jsonArray = "[1, 2, 3]";
        var content = new StringContent(jsonArray, Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(IEnumerable<int>));
        // Assert
        Assert.Equal(new List<int> { 1, 2, 3 }, result);
    }
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidEnumerableContent()
    {
        // Arrange
        var invalidJson = "\"invalid\"";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<JsonReaderException>(() =>
            _deserializer.DeserializeAsync(content, typeof(IEnumerable<int>)));
    }
}