using System;
using MyLab.ApiClient.Options;
using Newtonsoft.Json;

namespace MyLab.ApiClient.JsonSerialization
{
    /// <summary>
    /// A JSON serializer implementation using Newtonsoft.Json.
    /// </summary>
    /// <remarks>
    /// This class provides methods for serializing objects to JSON strings and deserializing JSON strings to objects.
    /// It utilizes <see cref="Newtonsoft.Json.JsonSerializerSettings"/> for configuring serialization behavior.
    /// </remarks>
    public class NewtonJsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// Provides a default instance of <see cref="NewtonJsonSerializer"/> configured with default JSON serialization settings.
        /// </summary>
        public static readonly NewtonJsonSerializer Default = new NewtonJsonSerializer(new ApiJsonSettings());
        
        readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of <see cref="NewtonJsonSerializer"/>
        /// </summary>
        public NewtonJsonSerializer(ApiJsonSettings settings)
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = settings.IgnoreNullValues
                    ? NullValueHandling.Ignore
                    : NullValueHandling.Include
            };
        }

        /// <inheritdoc />
        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be null or empty", nameof(json));

            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings)!;
        }

        /// <inheritdoc />
        public object Deserialize(string json, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be null or empty", nameof(json));

            return JsonConvert.DeserializeObject(json, targetType, _jsonSerializerSettings)!;
        }

        /// <inheritdoc />
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        }
    }
}
