using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Moq;
using MyLab.ApiClient.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyLab.ApiClient.Tests.Options;

[TestSubject(typeof(ApiClientOptions))]
public class ApiClientOptionsBehavior
{
    [Fact]
    public void ShouldExtractJsonSettingsFromConfiguration()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { ApiClientOptions.DefaultSectionName + ":JsonSettings:IgnoreNullValues", "false" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
        var section = configuration.GetSection(ApiClientOptions.DefaultSectionName);

        // Act
        var options = ApiClientOptions.ExtractFromSection(section);

        // Assert
        Assert.NotNull(options);
        Assert.NotNull(options.JsonSettings);
        Assert.False(options.JsonSettings.IgnoreNullValues);
    }
    
    [Fact]
    public void ShouldExtractApiUrlFormSettingsFromConfiguration()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { ApiClientOptions.DefaultSectionName + ":UrlFormSettings:EscapeSymbols", "false" },
            { ApiClientOptions.DefaultSectionName + ":UrlFormSettings:IgnoreNullValues", "true" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
        var section = configuration.GetSection(ApiClientOptions.DefaultSectionName);

        // Act
        var options = ApiClientOptions.ExtractFromSection(section);

        // Assert
        Assert.NotNull(options.UrlFormSettings);
        Assert.False(options.UrlFormSettings.EscapeSymbols);
        Assert.True(options.UrlFormSettings.IgnoreNullValues);
    }

    [Fact]
    public void ShouldExtractEndpointsFromConfigurationWithEndpointSections()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { ApiClientOptions.DefaultSectionName + ":Endpoints:ServiceA:Url", "https://service-a.com" },
            { ApiClientOptions.DefaultSectionName + ":Endpoints:ServiceA:SkipServerSslCertVerification", "true" },
            { ApiClientOptions.DefaultSectionName + ":Endpoints:ServiceB:Url", "https://service-b.com" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
        var section = configuration.GetSection(ApiClientOptions.DefaultSectionName);
        
        // Act
        var options = ApiClientOptions.ExtractFromSection(section);
        
        // Assert
        Assert.NotNull(options.Endpoints);
        Assert.Equal(2, options.Endpoints.Count);
        Assert.True(options.Endpoints.ContainsKey("ServiceA"));
        Assert.Equal("https://service-a.com", options.Endpoints["ServiceA"].Url);
        Assert.True(options.Endpoints["ServiceA"].SkipServerSslCertVerification);
        Assert.True(options.Endpoints.ContainsKey("ServiceB"));
        Assert.Equal("https://service-b.com", options.Endpoints["ServiceB"].Url);
        Assert.False(options.Endpoints["ServiceB"].SkipServerSslCertVerification);
    }

    [Fact]
    public void ShouldExtractEndpointsFromConfigurationWithoutEndpointSections()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { ApiClientOptions.DefaultSectionName + ":ServiceA", "https://service-a.com" },
            { ApiClientOptions.DefaultSectionName + ":ServiceB", "https://service-b.com" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
        var section = configuration.GetSection(ApiClientOptions.DefaultSectionName);

        // Act
        var options = ApiClientOptions.ExtractFromSection(section);

        // Assert
        Assert.NotNull(options.Endpoints);
        Assert.Equal(2, options.Endpoints.Count);
        Assert.True(options.Endpoints.ContainsKey("ServiceA"));
        Assert.Equal("https://service-a.com", options.Endpoints["ServiceA"].Url);
        Assert.False(options.Endpoints["ServiceA"].SkipServerSslCertVerification);
        Assert.True(options.Endpoints.ContainsKey("ServiceB"));
        Assert.Equal("https://service-b.com", options.Endpoints["ServiceB"].Url);
        Assert.False(options.Endpoints["ServiceB"].SkipServerSslCertVerification);
    }

    [Fact]
    public void ShouldThrowExceptionForNullSection()
    {
        // Arrange
        IConfigurationSection section = null;
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            ApiClientOptions.ExtractFromSection(section)
        );
        Assert.Equal("Value cannot be null. (Parameter 'section')", exception.Message);
    }
    
    [Fact]
    public void ShouldThrowExceptionForNonExistentSection()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var section = configuration.GetSection("NonExistentSection");
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ApiClientOptions.ExtractFromSection(section)
        );
        Assert.Equal("The section does not exists (Parameter 'section')", exception.Message);
    }
    
    [Fact]
    public void ShouldHandleEmptyEndpoints()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { ApiClientOptions.DefaultSectionName + ":JsonSettings:IgnoreNullValues", "true" },
            { ApiClientOptions.DefaultSectionName + ":UrlFormSettings:EscapeSymbols", "true" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
        var section = configuration.GetSection(ApiClientOptions.DefaultSectionName);
        // Act
        var options = ApiClientOptions.ExtractFromSection(section);
        // Assert
        Assert.NotNull(options);
        Assert.NotNull(options.JsonSettings);
        Assert.True(options.JsonSettings.IgnoreNullValues);
        Assert.NotNull(options.UrlFormSettings);
        Assert.True(options.UrlFormSettings.EscapeSymbols);
        Assert.NotNull(options.Endpoints);
        Assert.Empty(options.Endpoints);
    }
}