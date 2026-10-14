using System;

namespace MyLab.ApiClient.JsonSerialization;

/// <summary>
/// Provides methods for serializing objects to JSON and deserializing JSON strings to objects.
/// </summary>
public interface IJsonSerializer
{
    /// <summary>
    /// Deserializes the specified JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of type <typeparamref name="T"/> deserialized from the JSON string.</returns>
    /// <exception cref="System.ArgumentException">Thrown when the <paramref name="json"/> string is null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="Newtonsoft.Json.JsonException">Thrown when the JSON string is invalid or cannot be deserialized into the specified type.</exception>
    T Deserialize<T>(string json);

    /// <summary>
    /// Deserializes the specified JSON string into an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="targetType">The target type of the object to deserialize to.</param>
    /// <returns>An object of the specified <paramref name="targetType"/> deserialized from the JSON string.</returns>
    /// <exception cref="System.ArgumentException">Thrown when the <paramref name="json"/> string is null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="Newtonsoft.Json.JsonException">Thrown when the JSON string is invalid or cannot be deserialized into the specified type.</exception>
    object Deserialize(string json, Type targetType);
    
    /// <summary>
    /// Serializes the specified object into a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A JSON string representation of the specified object.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="obj"/> is null.</exception>
    string Serialize<T>(T obj);
}