using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

/// <summary>
/// Provides a mechanism to retrieve a content deserializer capable of processing a specific target type.
/// </summary>
/// <remarks>
/// This interface is designed to support the selection of an appropriate implementation of 
/// <see cref="IContentDeserializer"/> based on the target type for deserialization.
/// </remarks>
public interface IContentDeserializerProvider
{
    /// <summary>
    /// Retrieves a content deserializer that is capable of processing the specified target type.
    /// </summary>
    IContentDeserializer GetRequiredDeserializer(Type targetType);
}