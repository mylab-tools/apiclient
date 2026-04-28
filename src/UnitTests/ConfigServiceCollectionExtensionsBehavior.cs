using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.ApiClient;
using MyLab.ApiClient.Options;
using Xunit;

namespace UnitTests;

[TestSubject(typeof(ConfigServiceCollectionExtensions))]
public class ConfigServiceCollectionExtensionsBehavior
{
    [Theory]
    [InlineData(null, "sectionName")]
    [InlineData("config", null)]
    public void ShouldThrowArgumentNullExceptionForConfigureApiClientWithNullParams(string? config, string? sectionName)
    {
        //Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = config == null ? null : new ConfigurationBuilder().Build();

        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.ConfigureApiClient(configuration, sectionName));
    }

    [Fact]
    public void ShouldConfigureApiClientWithDefaultSectionName()
    {
        //Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Api:Endpoints:foo:Url", "http://foo.com")
            })
            .Build();

        //Act
        var sp = services.ConfigureApiClient(configuration).BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<ApiClientOptions>>().Value;

        //Assert
        Assert.NotNull(options);
        Assert.True(options.Endpoints.ContainsKey("foo"));
        Assert.Equal("http://foo.com", options.Endpoints["foo"].Url);
    }

    [Fact]
    public void ShouldThrowArgumentNullExceptionForConfigureApiClientWithNullAction()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.ConfigureApiClient((Action<ApiClientOptions>)null));
    }

    [Fact]
    public void ShouldConfigureApiClientWithCustomAction()
    {
        //Arrange
        var services = new ServiceCollection();

        //Act
        var sp = services.ConfigureApiClient(opt =>
        {
            opt.Endpoints.Add("foo", new ApiEndpointOptions { Url = "http://foo.com" });
        }).BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<ApiClientOptions>>().Value;

        //Assert
        Assert.NotNull(options);
        Assert.True(options.Endpoints.ContainsKey("foo"));
        Assert.Equal("http://foo.com", options.Endpoints["foo"].Url);
    }
}