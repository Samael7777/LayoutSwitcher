using System.Text.Json;
using System.Text.Json.Serialization;

namespace LayoutSwitcher.Control.Json;

public class KeyboardLayoutJsonConverter : JsonConverter<KeyboardLayout>
{
    public override KeyboardLayout Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hkl = 0u;

        if (reader.TokenType != JsonTokenType.StartObject)
            return KeyboardLayout.Empty;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName &&
                reader.GetString() == nameof(KeyboardLayout.Hkl))
            {
                reader.Read();
                hkl = reader.GetUInt32();
            }
            else
            {
                reader.Skip();
            }
        }

        return KeyboardLayout.GetLayout(hkl); // вернёт Empty при hkl == 0
    }

    public override void Write(Utf8JsonWriter writer, KeyboardLayout value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(nameof(KeyboardLayout.Hkl), value.Hkl);
        writer.WriteEndObject();
    }
}