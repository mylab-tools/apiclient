using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing
{
    /// <summary>
    /// Defines a contract for deserializing HTTP content into objects of a specified type.
    /// </summary>
    public interface IContentDeserializer
    {
        /// <summary>
        /// Determines whether the deserializer can process content for the specified return type.
        /// </summary>
        /// <param name="returnType">The type of the object to be deserialized.</param>
        /// <returns>
        /// <see langword="true"/> if the deserializer supports the specified return type; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="returnType"/> is <see langword="null"/>.</exception>
        bool Predicate(Type returnType);

        /// <summary>
        /// Asynchronously deserializes the provided HTTP content into an object of the specified type.
        /// </summary>
        Task<object?> DeserializeAsync(HttpContent content, Type returnType);
    }
}
