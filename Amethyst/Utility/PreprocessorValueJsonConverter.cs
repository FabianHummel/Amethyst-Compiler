using System.Text.Json;
using System.Text.Json.Serialization;

namespace Amethyst.Utility;

public class PreprocessorValueJsonConverter : JsonConverter<IPreprocessorValue>
{
    public override IPreprocessorValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new InvalidOperationException($"Deserialization of {nameof(IPreprocessorValue)} is not supported.");
    }

    public override void Write(Utf8JsonWriter writer, IPreprocessorValue value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.AbstractValue);
    }
}