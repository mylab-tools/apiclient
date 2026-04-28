using Microsoft.Extensions.Configuration;
using MyLab.ApiClient.JsonSerialization;
using MyLab.ApiClient.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLab.ApiClient.Options;

/// <summary>
/// Represents the configuration options for API clients, including endpoints, 
/// JSON serialization settings, and URL-encoded form serialization settings.
/// </summary>
public class ApiClientOptions
{
    /// <summary>
    /// Default configuration section name
    /// </summary>
    public const string DefaultSectionName = "Api";
    
    /// <summary>
    /// List of api connections options
    /// </summary>
    public Dictionary<string, ApiEndpointOptions> Endpoints { get; set; } = new();

    /// <summary>
    /// Defines JSON serializer
    /// </summary>
    public IJsonSerializer JsonSerializer { get; set; } = NewtonJsonSerializer.Default;

    /// <summary>
    /// Defines url-encoded-form serialization settings
    /// </summary>
    public ApiUrlFormSettings? UrlFormSettings { get; set; }
    
    /// <summary>
    /// Gets or sets the HTTP message dumper used to log or inspect HTTP requests and responses.
    /// </summary>
    public HttpMessageDumper? Dumper { get; set; }

    /// <summary>
    /// Fill options from configuration section 
    /// </summary>
    public static void FillFromSection(ApiClientOptions opt, IConfigurationSection section)
    {
        if (opt == null) throw new ArgumentNullException(nameof(opt));
        if (section == null) throw new ArgumentNullException(nameof(section));
        if (!section.Exists()) throw new ArgumentException("The section does not exists", nameof(section));

        opt.UrlFormSettings = ExtractUrlFormSettings(section.GetSection(nameof(UrlFormSettings)));

        var listSection = section.GetSection("List");
        if (listSection.Exists())
        {
            throw new NotSupportedException("Api/List section is no longer supported");
        }

        var endpointsSection = section.GetSection(nameof(Endpoints));

        var endpointSections = endpointsSection != null && endpointsSection.Exists()
            ? endpointsSection.GetChildren()
            : section.GetChildren().Where(s => s.Key != nameof(UrlFormSettings));

        ExtractEndpoints(opt.Endpoints, endpointSections);
    }

    /// <summary>
    /// Retrieves the API endpoint options from the specified configuration section.
    /// </summary>
    /// <param name="endpointSection">
    /// The configuration section containing the endpoint options. Must not be null and must exist.
    /// </param>
    /// <returns>
    /// An instance of <see cref="ApiEndpointOptions"/> populated with the settings from the configuration section.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="endpointSection"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="endpointSection"/> does not exist.
    /// </exception>
    public static ApiEndpointOptions GetEndpointOptions(IConfigurationSection endpointSection)
    {
        if (endpointSection == null) throw new ArgumentNullException(nameof(endpointSection));
        if (!endpointSection.Exists()) throw new ArgumentException("The section does not exists", nameof(endpointSection));

        var res = new ApiEndpointOptions();

        if (endpointSection.Value != null)
        {
            res.Url = endpointSection.Value;
        }
        else
        {
            endpointSection.Bind(res);
        }

        return res;
    }

    static void ExtractEndpoints(Dictionary<string, ApiEndpointOptions> optEndpoints, IEnumerable<IConfigurationSection> endpointSections)
    {
        foreach (var endpointSection in endpointSections)
        {
            var endpointSettings = GetEndpointOptions(endpointSection);
            optEndpoints.Add(endpointSection.Key, endpointSettings);
        }
    }

    static ApiUrlFormSettings? ExtractUrlFormSettings(IConfigurationSection? section)
    {
        if (section == null || !section.Exists()) return null;

        var s = new ApiUrlFormSettings();

        section.Bind(s);

        return s;
    }
}
