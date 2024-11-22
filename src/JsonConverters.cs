using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NcmSharp;
internal class IdConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                if (long.TryParse(reader.GetString(), out var value))
                    return value;
                return null;
            case JsonTokenType.Number:
                return reader.GetInt64();
            default:
                return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue((long)value);
    }
}
public class NMSLConverter : JsonConverter<string[][]>
{
    public override string[][] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new List<List<string>>();

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of array.");

        while (reader.Read())
        {
            var isEnd = false;
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    {
                        var innerList = new List<string>();

                        while (reader.Read())
                        {
                            var isInnerArrayEnd = false;
                            switch (reader.TokenType)
                            {
                                case JsonTokenType.String:
                                    innerList.Add(reader.GetString() ?? string.Empty);
                                    break;
                                case JsonTokenType.Number:
                                    innerList.Add(reader.GetDouble().ToString());
                                    break;
                                case JsonTokenType.EndArray:
                                    isInnerArrayEnd = true;
                                    break;
                                default:
                                    throw new JsonException($"Unexpected token type {reader.TokenType} in inner array.");
                            }
                            if (isInnerArrayEnd)
                                break;
                        }
                        result.Add(innerList);
                    }
                    break;
                case JsonTokenType.EndArray:
                    isEnd = true;
                    break;
                default:
                    throw new JsonException("Expected start of inner array.");
            }
            if (isEnd)
                break;
        }

        return result.Select(inner => inner.ToArray()).ToArray();
    }

    public override void Write(Utf8JsonWriter writer, string[][] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var innerArray in value)
        {
            writer.WriteStartArray();

            foreach (var item in innerArray)
                writer.WriteStringValue(item);

            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }
}
