using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.ApiClient;
using MyLab.ApiClient.Options;
using Xunit;

namespace MyLab.ApiClient.Tests;

[TestSubject(typeof(ServiceCollectionExtensions))]
public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void ShouldThrowArgumentNullExceptionWhenConfigIsNull()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        IConfiguration config = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.ConfigureApiClient(config));
    }

    [Fact]
    public void ShouldThrowArgumentNullExceptionWhenSectionNameIsNull()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        IConfiguration config = new ConfigurationBuilder().Build();
        string sectionName = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.ConfigureApiClient(config, sectionName));
    }

    [Fact]
    public void ShouldConfigureApiClientOptionsWithDefaultSectionName()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { $"{ApiClientOptions.DefaultSectionName}:Endpoints:Test:Url", "http://example.com" }
            })
            .Build();

        // Act
        services.ConfigureApiClient(config);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<ApiClientOptions>>()?.Value;

        Assert.NotNull(options);
        Assert.True(options.Endpoints.ContainsKey("Test"));
        Assert.Equal("http://example.com", options.Endpoints["Test"].Url);
    }

    [Fact]
    public void ShouldConfigureApiClientOptionsWithCustomSectionName()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "CustomSection:Endpoints:Test:Url", "http://example.com" }
            })
            .Build();

        // Act
        services.ConfigureApiClient(config, "CustomSection");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<ApiClientOptions>>()?.Value;

        Assert.NotNull(options);
        Assert.True(options.Endpoints.ContainsKey("Test"));
        Assert.Equal("http://example.com", options.Endpoints["Test"].Url);
    }

    [Fact]
    public void ShouldThrowArgumentNullExceptionWhenConfigureOptionsIsNull()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        Action<ApiClientOptions> configureOptions = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.ConfigureApiClient(configureOptions));
    }

    [Fact]
    public void ShouldConfigureApiClientOptionsWithCustomAction()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        var customOptions = new ApiClientOptions
        {
            Endpoints = new Dictionary<string, ApiEndpointOptions>
            {
                { "Test", new ApiEndpointOptions { Url = "http://example.com" } }
            }
        };

        // Act
        services.ConfigureApiClient(options => { options.Endpoints = customOptions.Endpoints; });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<ApiClientOptions>>()?.Value;

        Assert.NotNull(options);
        Assert.True(options.Endpoints.ContainsKey("Test"));
        Assert.Equal("http://example.com", options.Endpoints["Test"].Url);
    }
}