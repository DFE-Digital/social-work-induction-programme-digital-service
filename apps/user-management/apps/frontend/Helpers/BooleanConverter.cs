using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dfe.Sww.Ecf.Frontend.Helpers;

public class BooleanConverter() : JsonConverter<bool>
{
    private static readonly JsonException BooleanParsingException =
        new(
            "The boolean property could not be read as a valid boolean"
                + " json value or parsed from boolean string value (e.g. 'true'/'false')."
        );

    public override bool Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) =>
        reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.String
                => reader.GetString() switch
                {
                    var value
                        when string.Equals(
                            value,
                            bool.TrueString,
                            StringComparison.OrdinalIgnoreCase
                        )
                        => true,
                    var value
                        when string.Equals(
                            value,
                            bool.FalseString,
                            StringComparison.OrdinalIgnoreCase
                        )
                        => false,
                    _ => throw BooleanParsingException
                },
            _ => throw BooleanParsingException
        };

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
        writer.WriteBooleanValue(value);
}
