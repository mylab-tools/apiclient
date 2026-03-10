using System.Collections.ObjectModel;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class SupportedContentDeserializers : ReadOnlyCollection<IContentDeserializer>
{
    public static readonly SupportedContentDeserializers Instance = new ();

    SupportedContentDeserializers() : base(Create())
    {
    }

    static IContentDeserializer[] Create() => [
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
        new StructuredObjectContentDeserializer(),
        new EnumerableContentDeserializer()
    ];
}