using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameOfLife.Common.Converters;

public class ByteArrayAsNumbersConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected JSON array for byte[] deserialization, but received {reader.TokenType}.");

        var list = new List<byte>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (!reader.TryGetByte(out var value))
                throw new JsonException($"Value is not a valid byte (0-255) at token type {reader.TokenType}.");
            list.Add(value);
        }

        return [.. list];
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var b in value)
            writer.WriteNumberValue(b);
        writer.WriteEndArray();
    }
}
