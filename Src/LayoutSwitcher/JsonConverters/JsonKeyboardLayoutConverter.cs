using System.Text.Json;
using System.Text.Json.Serialization;
using LayoutControl;

namespace LayoutSwitcher.JsonConverters;

public class JsonKeyboardLayoutConverter : JsonConverter<KeyboardLayout>
{
    public override KeyboardLayout Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();
        
        var hkl = 0u;
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    return KeyboardLayout.GetLayout(hkl);
                case JsonTokenType.PropertyName:
                    if (reader.GetString() != nameof(KeyboardLayout.Hkl))
                        throw new JsonException();
                    reader.Read();
                    hkl = reader.GetUInt32();
                    break;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, KeyboardLayout value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(nameof(KeyboardLayout.Hkl), value.Hkl);
        writer.WriteEndObject();
    }
}