using System;
using System.Text.Json;
using MyLab.ApiClient.Options;

namespace MyLab.ApiClient.JsonSerialization;

/// <summary>
/// A JSON serializer implementation using the Microsoft JSON library.
/// </summary>
/// <remarks>
/// This class provides methods for serializing objects to JSON strings and deserializing JSON strings to objects.
/// It is configured using the <see cref="MyLab.ApiClient.Options.ApiJsonSettings"/> class to customize serialization behavior.
/// </remarks>
public class MicrosoftJsonSerializer : IJsonSerializer
{
    readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="MicrosoftJsonSerializer"/>
    /// </summary>
    public MicrosoftJsonSerializer(ApiJsonSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
            
        _options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = settings.IgnoreNullValues
                ? System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                : System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };
    }

    /// <inheritdoc />
    public T Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null, empty, or consist only of white-space characters.",
                nameof(json));

        return JsonSerializer.Deserialize<T>(json, _options)!;
    }

    /// <inheritdoc />
    public object Deserialize(string json, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null, empty, or consist only of white-space characters.",
                nameof(json));

        if (targetType == null) throw new ArgumentNullException(nameof(targetType), "Target type cannot be null.");

        return JsonSerializer.Deserialize(json, targetType, _options)!;
    }

    /// <inheritdoc />
    public string Serialize<T>(T obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj), "Object to serialize cannot be null.");

        return JsonSerializer.Serialize(obj, _options);
    }
}