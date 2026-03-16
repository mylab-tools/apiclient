using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class SupportedContentDeserializers : ReadOnlyCollection<IContentDeserializer>, IContentDeserializerProvider
{
    SupportedContentDeserializers(IContentDeserializer[] deserializers) : base(deserializers)
    {
    }

    public static SupportedContentDeserializers Create(JsonDeserializationTools jsonDesTools, XmlDeserializationTools xmlDesTools)
    {
        return new SupportedContentDeserializers(CreateArray(jsonDesTools, xmlDesTools));
    }

    static IContentDeserializer[] CreateArray(JsonDeserializationTools jsonDesTools, XmlDeserializationTools xmlDesTools) => [
        new BoolContentDeserializer(),
        new ShortContentDeserializer(),
        new UShortContentDeserializer(),
        new IntContentDeserializer(),
        new UintContentDeserializer(),
        new LongContentDeserializer(),
        new ULongContentDeserializer(),
        new DoubleContentDeserializer(),
        new FloatContentDeserializer(),
        new DecimalContentDeserializer(),
        new StringContentDeserializer(),
        new TimeSpanContentDeserializer(),
        new DateTimeContentDeserializer(),
        new GuidContentDeserializer(),
        new BinaryContentDeserializer(),
        new ProblemDetailsContentDeserializer(jsonDesTools),
        new ValidationProblemDetailsContentDeserializer(jsonDesTools),
        new StructuredObjectContentDeserializer(jsonDesTools, xmlDesTools),
        new EnumerableContentDeserializer(jsonDesTools, xmlDesTools),
    ];

    public IContentDeserializer GetRequiredDeserializer(Type targetType)
    {
        var deserializer = this.FirstOrDefault(p => p.Predicate(targetType));
        if (deserializer == null)
            throw new NotSupportedException(
                $"Content deserializer not found for type '{targetType.FullName}'");

        return deserializer;
    }
}