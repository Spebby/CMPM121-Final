using System;
using Newtonsoft.Json;


namespace CMPM.Utils {
    public class SecondsParser : JsonConverter<int> {
        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue,
                                     JsonSerializer serializer) {
            double seconds;
            if (reader.TokenType == JsonToken.String) {
                return !double.TryParse((string)reader.Value, out seconds)
                    ? throw new JsonSerializationException($"Invalid string format for delay: {reader.Value}")
                    : (int)(seconds * 1000);
            }

            seconds = reader.TokenType is JsonToken.Float or JsonToken.Integer
                ? Convert.ToDouble(reader.Value)
                : throw new JsonSerializationException(
                    $"Unexpected token type {reader.TokenType} for delay. Expected string, float, or int.");

            return (int)(seconds * 1000);
        }

        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer) {
            writer.WriteValue(value / 1000.0); // write back as seconds
        }
    }
}