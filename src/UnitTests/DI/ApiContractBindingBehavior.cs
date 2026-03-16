using System;
using JetBrains.Annotations;
using MyLab.ApiClient.DI;
using MyLab.ApiClient.Options;
using Xunit;

namespace MyLab.ApiClient.Tests.DI;

[TestSubject(typeof(ApiContractBinding))]
public partial class ApiContractBindingBehavior
{
    [Fact]
    public void ShouldThrowArgumentNullException_WhenContractTypeIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApiContractBinding(null!));
    }

    [Theory]
    [MemberData(nameof(GetBindingKeysCases))]
    public void ShouldCreateCorrectBindingKeys(string expectedKey)
    {
        // Arrange
        var contractType = typeof(ITestContractWithCustomBinding);

        // Act
        var binding = new ApiContractBinding(contractType);

        // Assert
        Assert.Contains(expectedKey, binding.Keys!);
    }

    [Fact]
    public void ShouldReturnNull_WhenNoMatchingEndpointOptions()
    {
        // Arrange
        var contractType = typeof(ITestContract);
        var binding = new ApiContractBinding(contractType);
        var clientOptions = new ApiClientOptions();

        // Act
        var found = binding.TryGetOptions(clientOptions, out var result, out _);

        // Assert
        Assert.False(found);
        Assert.Null(result);
    }

    [Fact]
    public void ShouldReturnCorrectEndpointOptions_WhenMatchingKeyExists()
    {
        // Arrange
        var contractType = typeof(ITestContract);
        var binding = new ApiContractBinding(contractType);
        var clientOptions = new ApiClientOptions
        {
            Endpoints =
            {
                { "ITestContract", new ApiEndpointOptions { Url = "http://test.com" } }
            }
        };

        // Act
        var found = binding.TryGetOptions(clientOptions, out var opts,out var bindingKey);

        // Assert
        Assert.True(found);
        Assert.NotNull(opts);
        Assert.Equal("http://test.com", opts.Url);
        Assert.Equal("ITestContract", bindingKey);
    }

    [Fact]
    public void ShouldHandleEmptyEndpointsGracefully()
    {
        // Arrange
        var contractType = typeof(ITestContract);
        var binding = new ApiContractBinding(contractType);
        var clientOptions = new ApiClientOptions();

        // Act
        var found = binding.TryGetOptions(clientOptions, out var result, out _);

        // Assert
        Assert.False(found);
        Assert.Null(result);
    }

    [Fact]
    public void ShouldIncludeFullNameInBindingKeys_WhenFullNameIsAvailable()
    {
        // Arrange
        var contractType = typeof(ITestContractWithFullName);

        // Act
        var binding = new ApiContractBinding(contractType);

        // Assert
        Assert.Contains(contractType.FullName, binding.Keys!);
    }

    [Fact]
    public void ShouldIncludeCustomBindingInBindingKeys_WhenAttributeIsPresent()
    {
        // Arrange
        var contractType = typeof(ITestContractWithCustomBinding);

        // Act
        var binding = new ApiContractBinding(contractType);

        // Assert
        Assert.Contains("CustomBinding", binding.Keys!);
    }

    // Helper classes for testing
}