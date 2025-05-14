using System;
using Newtonsoft.Json;
using static CMPM.Level.SpawnPoint;


namespace CMPM.Utils.SpawningParsers {
    public class SpawnLocationParser : JsonConverter<SpawnName> {
        public override SpawnName ReadJson(JsonReader reader, Type objectType, SpawnName existingValue,
                                           bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null || reader.Value == null) return SpawnName.RANDOM;

            string raw = (reader.Value as string)?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(raw) || raw == "random")
                return SpawnName.RANDOM;

            string[] parts = raw.Split(' ');
            if (parts.Length == 2 && parts[0] == "random") {
                return parts[1] switch {
                    "red"   => SpawnName.RED,
                    "green" => SpawnName.GREEN,
                    "bone"  => SpawnName.BONE,
                    _       => throw new ArgumentException($"Unknown spawn type in location: '{raw}'")
                };
            }

            throw new FormatException($"Invalid location format: '{raw}'. Expected 'random' or 'random <type>'.");
        }

        public override void WriteJson(JsonWriter writer, SpawnName value, JsonSerializer serializer) {
            // Optional: convert enum back to string if you're writing JSON later
            string output = value switch {
                SpawnName.RANDOM => "random",
                SpawnName.RED    => "random red",
                SpawnName.GREEN  => "random green",
                SpawnName.BONE   => "random bone",
                _                => throw new ArgumentOutOfRangeException(nameof(value))
            };
            writer.WriteValue(output);
        }
    }
}