using Microsoft.Extensions.Configuration;
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
    /// Defines JSON serialization settings
    /// </summary>
    public ApiJsonSettings? JsonSettings { get; set; }

    /// <summary>
    /// Defines url-encoded-form serialization settings
    /// </summary>
    public ApiUrlFormSettings? UrlFormSettings { get; set; }

    /// <summary>
    /// Extracts options from configuration section 
    /// </summary>
    public static ApiClientOptions ExtractFromSection(IConfigurationSection section)
    {
        if (section == null) throw new ArgumentNullException(nameof(section));
        if (!section.Exists()) throw new ArgumentException("The section does not exists", nameof(section));

        var opt = new ApiClientOptions
        {
            JsonSettings = ExtractJsonSettings(section.GetSection(nameof(JsonSettings))),
            UrlFormSettings = ExtractUrlFormSettings(section.GetSection(nameof(UrlFormSettings)))
        };

        var listSection = section.GetSection("List");
        if (listSection.Exists())
        {
            throw new NotSupportedException("Api/List section is no longer supported");
        }

        var endpointsSection = section.GetSection(nameof(Endpoints));

        var endpointSections = endpointsSection != null && endpointsSection.Exists()
            ? endpointsSection.GetChildren()
            : section.GetChildren().Where(s => s.Key != nameof(JsonSettings) && s.Key != nameof(UrlFormSettings));

        ExtractEndpoints(opt.Endpoints, endpointSections);

        return opt;
    }

    static void ExtractEndpoints(Dictionary<string, ApiEndpointOptions> optEndpoints, IEnumerable<IConfigurationSection> endpointSections)
    {
        foreach (var endpointSection in endpointSections)
        {
            var endpointSettings = GetEndpointOptions(endpointSection);
            optEndpoints.Add(endpointSection.Key, endpointSettings);
        }
    }

    static ApiEndpointOptions GetEndpointOptions(IConfigurationSection endpointSection)
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

    static ApiUrlFormSettings? ExtractUrlFormSettings(IConfigurationSection? section)
    {
        if (section == null || !section.Exists()) return null;

        var s = new ApiUrlFormSettings();

        section.Bind(s);

        return s;
    }

    static ApiJsonSettings? ExtractJsonSettings(IConfigurationSection? section)
    {
        if (section == null || !section.Exists()) return null;

        var s = new ApiJsonSettings();

        section.Bind(s);

        return s;
    }
}
