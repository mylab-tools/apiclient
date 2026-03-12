using System;
using JetBrains.Annotations;
using MyLab.ApiClient.DI;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using Xunit;

namespace MyLab.ApiClient.Tests.DI;

[TestSubject(typeof(ApiContractBinding))]
public class ApiContractBindingBehavior
{
    [Fact]
    public void ShouldThrowArgumentNullException_WhenContractTypeIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApiContractBinding(null!));
    }

    [Theory]
    [InlineData("ContractName", "ContractName")]
    [InlineData("ContractName", "Namespace.ContractName")]
    [InlineData("ContractName", "CustomBinding")]
    public void ShouldCreateCorrectBindingKeys(string contractName, string expectedKey)
    {
        // Arrange
        var contractType = typeof(ITestContractWithCustomBinding);
        var binding = new ApiContractBinding(contractType);

        // Act
        var keys = binding.GetType()
            .GetField("_contractBindingKeys",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(binding) as string[];

        // Assert
        Assert.Contains(expectedKey, keys!);
    }

    [Fact]
    public void ShouldReturnNull_WhenNoMatchingEndpointOptions()
    {
        // Arrange
        var contractType = typeof(ITestContract);
        var binding = new ApiContractBinding(contractType);
        var clientOptions = new ApiClientOptions();

        // Act
        var result = binding.GetOptions(clientOptions);

        // Assert
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
        var result = binding.GetOptions(clientOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("http://test.com", result!.Url);
    }

    [Fact]
    public void ShouldHandleEmptyEndpointsGracefully()
    {
        // Arrange
        var contractType = typeof(ITestContract);
        var binding = new ApiContractBinding(contractType);
        var clientOptions = new ApiClientOptions();

        // Act
        var result = binding.GetOptions(clientOptions);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ShouldIncludeFullNameInBindingKeys_WhenFullNameIsAvailable()
    {
        // Arrange
        var contractType = typeof(ITestContractWithFullName);
        var binding = new ApiContractBinding(contractType);

        // Act
        var keys = binding.GetType()
            .GetField("_contractBindingKeys",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(binding) as string[];

        // Assert
        Assert.Contains("Namespace.ITestContractWithFullName", keys!);
    }

    [Fact]
    public void ShouldIncludeCustomBindingInBindingKeys_WhenAttributeIsPresent()
    {
        // Arrange
        var contractType = typeof(ITestContractWithCustomBinding);
        var binding = new ApiContractBinding(contractType);

        // Act
        var keys = binding.GetType()
            .GetField("_contractBindingKeys",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(binding) as string[];

        // Assert
        Assert.Contains("CustomBinding", keys!);
    }

    // Helper classes for testing
    interface ITestContract
    {
    }

    [ApiContract(Binding = "CustomBinding")]
    interface ITestContractWithCustomBinding
    {
    }

    interface ITestContractWithFullName
    {
    }
}