using System;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient.Problems;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class ProblemDetailsContentDeserializer : IContentDeserializer
{
    readonly JsonDeserializationTools _jsonSerTools;

    public ProblemDetailsContentDeserializer(JsonDeserializationTools jsonSerTools)
    {
        _jsonSerTools = jsonSerTools;
    }

    public bool Predicate(Type returnType)
    {
        return returnType == typeof(ProblemDetails);
    }

    public async Task<object?> DeserializeAsync(HttpContent content, Type returnType)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));

        return await new MediaTypeProc(content)
            .Supports("application/problem+json", async c => await _jsonSerTools.ReadObjectJson(c, returnType))
            .GetResultAsync();
    }
}