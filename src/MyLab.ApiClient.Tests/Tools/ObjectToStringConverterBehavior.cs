using System;
using System.Globalization;
using JetBrains.Annotations;
using MyLab.ApiClient.Tools;
using Xunit;

namespace MyLab.ApiClient.Tests.Tools;

[TestSubject(typeof(ObjectToStringConverter))]
public class ObjectToStringConverterBehavior
{
    [Theory]
    [InlineData(null, "")]
    [InlineData(123, "123")]
    [InlineData("test", "test")]
    [InlineData(123.45, "123.45")]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    public void ShouldConvertBasicTypesToString(object? input, string expected)
    {
        // Act
        var result = ObjectToStringConverter.ToString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2023-10-01T12:34:56", "2023-10-01T12:34:56")]
    [InlineData("2000-01-01T00:00:00", "2000-01-01T00:00:00")]
    public void ShouldConvertDateTimeToString(string dateTimeString, string expected)
    {
        // Arrange
        var dateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);

        // Act
        var result = ObjectToStringConverter.ToString(dateTime);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("d2719b6e-3f4b-4a6b-8a3e-2b7f5c3f8a9d", "d2719b6e3f4b4a6b8a3e2b7f5c3f8a9d")]
    [InlineData("00000000-0000-0000-0000-000000000000", "00000000000000000000000000000000")]
    public void ShouldConvertGuidToString(string guidString, string expected)
    {
        // Arrange
        var guid = Guid.Parse(guidString);

        // Act
        var result = ObjectToStringConverter.ToString(guid);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ShouldHandleEmptyString()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = ObjectToStringConverter.ToString(input);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ShouldHandleSpecialCharacters()
    {
        // Arrange
        var input = "!@#$%^&*()";

        // Act
        var result = ObjectToStringConverter.ToString(input);

        // Assert
        Assert.Equal("!@#$%^&*()", result);
    }

    [Fact]
    public void ShouldHandleLargeNumbers()
    {
        // Arrange
        var input = long.MaxValue;

        // Act
        var result = ObjectToStringConverter.ToString(input);

        // Assert
        Assert.Equal(long.MaxValue.ToString(), result);
    }

    [Fact]
    public void ShouldHandleNegativeNumbers()
    {
        // Arrange
        var input = -12345;

        // Act
        var result = ObjectToStringConverter.ToString(input);

        // Assert
        Assert.Equal("-12345", result);
    }
}