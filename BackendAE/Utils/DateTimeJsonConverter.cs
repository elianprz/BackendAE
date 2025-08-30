using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string Format = "dd-MM-yyyy";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;
        throw new JsonException($"La fecha debe tener el formato {Format}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}

public class NullableDateTimeJsonConverter : JsonConverter<DateTime?>
{
    private const string Format = "dd-MM-yyyy";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrEmpty(value)) return null;
        if (DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;
        throw new JsonException($"La fecha debe tener el formato {Format}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString(Format));
        else
            writer.WriteNullValue();
    }
}