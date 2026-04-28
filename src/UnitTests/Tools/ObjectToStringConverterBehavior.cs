using System;
using System.Globalization;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using MyLab.ApiClient.Tools;
using Xunit;

namespace UnitTests.Tools;

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
    [InlineData("2023-10-01T12:34:56", "2023-10-01T12:34:56.0000000")]
    [InlineData("2000-01-01T00:00:00", "2000-01-01T00:00:00.0000000")]
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

    [Fact]
    public void ShouldConvertDateOnlyToString()
    {
        // Arrange
        var date = new DateOnly(2023, 10, 5); // Example date
        // Act
        var result = ObjectToStringConverter.ToString(date);
        // Assert
        Assert.Equal("2023-10-05", result); // Expected RFC3339 format for DateOnly
    }
    [Fact]
    public void ShouldConvertTimeOnlyToString()
    {
        // Arrange
        var time = new TimeOnly(14, 30, 15); // Example time: 14:30:15
        // Act
        var result = ObjectToStringConverter.ToString(time);
        // Assert
        Assert.Equal("14:30:15.0000000", result); // Expected RFC3339 format for TimeOnly
    }

    [Theory]
    [InlineData(TestEnum.ValueOne, "Value_One")]
    [InlineData(TestEnum.ValueTwo, "ValueTwo")]
    [InlineData(TestEnum.ValueThree, "ValueThree")]
    public void ShouldConvertEnumWithEnumMemberAttributeToString(TestEnum input, string expected)
    {
        // Act
        var result = ObjectToStringConverter.ToString(input);
        // Assert
        Assert.Equal(expected, result);
    }

    public enum TestEnum
    {
        [EnumMember(Value = "Value_One")]
        ValueOne,
        ValueTwo,
        [EnumMember]
        ValueThree
    }
}