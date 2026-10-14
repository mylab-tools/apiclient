using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(IntContentDeserializer))]
public class IntContentDeserializerBehavior
{
    readonly IntContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidIntFromJsonContent()
    {
        var content = new StringContent("123", Encoding.UTF8, "application/json");
        var result = await _deserializer.DeserializeAsync(content, typeof(int));
        Assert.Equal(123, result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidIntContent()
    {
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(int)));
    }
}