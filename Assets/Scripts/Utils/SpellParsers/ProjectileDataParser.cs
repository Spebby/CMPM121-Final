using System;
using CMPM.Spells;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.SpellParsers {
    public class ProjectileDataParser : JsonConverter<ProjectileData> {
        public override ProjectileData ReadJson(JsonReader reader, Type objectType, ProjectileData existingValue,
                                                bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            ProjectileType trajectory  = ProjectileManager.StringToProjectileType(obj["trajectory"]?.ToString());
            string         speedStr    = obj["speed"]?.ToString() ?? throw new JsonException("Missing 'speed'");
            string         hitCapStr   = obj["hitcap"]?.ToString() ?? throw new JsonException("Missing 'hitcap'");
            int            sprite      = obj["sprite"]?.ToObject<int>() ?? throw new JsonException("Missing 'sprite'");
            string         lifetimeStr = obj["lifetime"]?.ToString();

            if (speedStr == null) {
                throw new JsonException("Missing 'speed'");
            }

            RPNString  speed    = new(speedStr);
            RPNString  hitcap   = new(hitCapStr);
            RPNString? lifetime = string.IsNullOrEmpty(lifetimeStr) ? null : new RPNString(lifetimeStr);

            return new ProjectileData(trajectory, speed, hitcap, sprite, lifetime);
        }

        public override void WriteJson(JsonWriter writer, ProjectileData value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("trajectory");
            writer.WriteValue(value.Trajectory);
            writer.WritePropertyName("speed");
            writer.WriteValue(value.Speed.String);
            writer.WritePropertyName("hitcap");
            writer.WriteValue(value.HitCap.String);
            writer.WritePropertyName("sprite");
            writer.WriteValue(value.Sprite);
            if (value.Lifetime.HasValue) {
                writer.WritePropertyName("lifetime");
                writer.WriteValue(value.Lifetime.Value.String);
            }

            writer.WriteEndObject();
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}