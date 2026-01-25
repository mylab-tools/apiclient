using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring.ContentFactoring;

[TestSubject(typeof(XmlHttpContentFactory))]
public class XmlHttpContentFactoryBehavior
{
    [Fact]
    public async Task ShouldCreateEmptyContentWhenSourceIsNull()
    {
        // Arrange
        var factory = new XmlHttpContentFactory();

        // Act
        var result = factory.Create(null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.NotNull(result.Headers.ContentType);
        Assert.Equal("application/xml", result.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task ShouldCreateContentForSimpleObject()
    {
        // Arrange
        var factory = new XmlHttpContentFactory();
        var source = new TestModel<int> { Name = "Test", Value = 123 };

        // Act
        var result = factory.Create(source, null);

        // Assert
        Assert.NotNull(result);
        var content = await result.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("<Name>Test</Name>", content);
        Assert.Contains("<Value>123</Value>", content);
        Assert.NotNull(result.Headers.ContentType);
        Assert.Equal("application/xml", result.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task ShouldHandleComplexObjectSerialization()
    {
        // Arrange
        var factory = new XmlHttpContentFactory();
        var source = new HolderTestModel
        {
            Name = "Complex",
            Nested = new NestedTestModel { InnerName = "Inner", InnerValue = 456 }
        };

        // Act
        var result = factory.Create(source, null);

        // Assert
        Assert.NotNull(result);
        var content = await result.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("<Name>Complex</Name>", content);
        Assert.Contains("<Nested>", content);
        Assert.Contains("<InnerName>Inner</InnerName>", content);
        Assert.Contains("<InnerValue>456</InnerValue>", content);
        Assert.NotNull(result.Headers.ContentType);
        Assert.Equal("application/xml", result.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task ShouldHandleEmptyObjectSerialization()
    {
        // Arrange
        var factory = new XmlHttpContentFactory();
        var source = new TestModel<string>();

        // Act
        var result = factory.Create(source, null);

        // Assert
        Assert.NotNull(result);
        var content = await result.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal(
            "<TestModelOfString />",
            content);
        Assert.NotNull(result.Headers.ContentType);
        Assert.Equal("application/xml", result.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task ShouldHandleSpecialCharactersInObject()
    {
        // Arrange
        var factory = new XmlHttpContentFactory();
        var source = new TestModel<string> { Value = "Special <>&'\" Characters" };

        // Act
        var result = factory.Create(source, null);

        // Assert
        Assert.NotNull(result);
        var content = await result.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("&lt;", content);
        Assert.Contains("&gt;", content);
        Assert.Contains("&amp;", content);
        Assert.NotNull(result.Headers.ContentType);
        Assert.Equal("application/xml", result.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task ShouldHandleNullValuesInObject()
    {
        // Arrange
        var factory = new XmlHttpContentFactory();
        var source = new TestModel<string> { Name = "Test", Value = null };

        // Act
        var result = factory.Create(source, null);

        // Assert
        Assert.NotNull(result);
        var content = await result.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("<Name>Test</Name>", content);
        Assert.DoesNotContain("<Value>", content);
        Assert.NotNull(result.Headers.ContentType);
        Assert.Equal("application/xml", result.Headers.ContentType.MediaType);
    }

    [Fact]
    public void ShouldCalculateContentLength()
    {
        // Arrange
        var factory = new XmlHttpContentFactory();
        var source = new TestModel<string> { Name = "LengthTest" };

        // Act
        var result = factory.Create(source, null);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Headers.ContentLength > 0);
    }

    public class TestModel<T>
    {
        public string? Name { get; set; }
        
        public T? Value { get; set; }
    }

    public class HolderTestModel
    {
        public string? Name { get; set; }

        public NestedTestModel? Nested { get; set; }
    }

    public class NestedTestModel
    {
        public string? InnerName { get; set; }

        public int? InnerValue { get; set; }
    }
}