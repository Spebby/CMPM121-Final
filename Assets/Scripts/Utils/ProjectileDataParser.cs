using System;
using System.Globalization;
using CMPM.Spells;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils {
    public class ProjectileDataParser : JsonConverter<ProjectileData> {
        public override ProjectileData ReadJson(JsonReader reader, Type objectType, ProjectileData existingValue,
                                                bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            string trajectory  = obj["trajectory"]?.ToString();
            string speedStr    = obj["speed"]?.ToString();
            int    sprite      = obj["sprite"]?.ToObject<int>() ?? throw new JsonException("Missing 'sprite'");
            string lifetimeStr = obj["lifetime"]?.ToString();

            if (trajectory == null || speedStr == null) {
                throw new JsonException("Missing 'trajectory' or 'speed'");
            }

            if (!float.TryParse(speedStr, out float speed)) {
                throw new JsonException("Invalid 'speed' format");
            }

            uint lifetime = 0;
            if (lifetimeStr == null) return new ProjectileData(trajectory, speed, sprite, lifetime);
            lifetime = float.TryParse(lifetimeStr, out float parsed)
                ? (uint)(parsed * 1000)
                : throw new JsonException("Invalid 'lifetime' format");

            return new ProjectileData(trajectory, speed, sprite, lifetime);
        }

        public override void WriteJson(JsonWriter writer, ProjectileData value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("trajectory");
            writer.WriteValue(value.Trajectory);
            writer.WritePropertyName("speed");
            writer.WriteValue(value.Speed.ToString(CultureInfo.CurrentCulture));
            writer.WritePropertyName("sprite");
            writer.WriteValue(value.Sprite);
            if (value.Lifetime != 0) {
                writer.WritePropertyName("lifetime");
                writer.WriteValue((value.Lifetime / 1000.0).ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteEndObject();
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}