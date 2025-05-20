using System;
using CMPM.Relics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.RelicParsers {
    public class RelicDataParser : JsonConverter<RelicData> {
        public override void WriteJson(JsonWriter writer, RelicData value, JsonSerializer serializer) {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteValue(value.Name);

            writer.WritePropertyName("sprite");
            writer.WriteValue(value.SpriteIndex);

            writer.WritePropertyName("precondition");
            writer.WriteStartObject();
            writer.WritePropertyName("description");
            writer.WriteValue(value.Precondition.Description);
            writer.WritePropertyName("type");
            serializer.Serialize(writer, value.Precondition.Type);
            writer.WriteEndObject();

            writer.WritePropertyName("effect");
            writer.WriteStartObject();
            writer.WritePropertyName("description");
            writer.WriteValue(value.Effect.Description);
            writer.WritePropertyName("type");
            serializer.Serialize(writer, value.Effect.Type);
            writer.WritePropertyName("amount");
            writer.WriteValue(value.Effect.Amount.String);
            if (value.Effect.Expiration != EffectExpiration.None) {
                writer.WritePropertyName("until");
                serializer.Serialize(writer, value.Effect.Expiration);
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        public override RelicData ReadJson(JsonReader reader, Type objectType, RelicData existingValue,
                                           bool hasExistingValue,
                                           JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            string name   = obj["name"]?.ToString()        ?? throw new JsonException("Missing name");
            int    sprite = obj["sprite"]?.ToObject<int>() ?? throw new JsonException("Missing sprite");

            // Parse Precondition
            JObject pre = obj["precondition"]?.ToObject<JObject>() ?? throw new JsonException("Missing precondition");
            RelicData.RelicPreconditionData relicPrecondition = new(
                pre["description"]?.ToString() ?? throw new JsonException("Missing precondition.description"),
                pre["type"]?.ToObject<PreconditionType>(serializer) ?? throw new JsonException("Invalid precondition.type"),
                pre["amount"]?.ToObject<RPNString>(serializer) ?? throw new JsonException("Invalid precondition.amount")
            );

            // Parse Effect
            JObject eff = obj["effect"]?.ToObject<JObject>() ?? throw new JsonException("Missing effect");
            RelicData.RelicEffectData effect = new(
                eff["description"]?.ToString() ?? throw new JsonException("Missing effect.description"),
                eff["type"]?.ToObject<EffectType>(serializer) ?? throw new JsonException("Invalid effect.type"),
                new RPNString(eff["amount"]?.ToString() ?? throw new JsonException("Missing effect.amount")),
                eff["until"]?.ToObject<EffectExpiration>(serializer)
            );

            return new RelicData(name, sprite, relicPrecondition, effect);
        }
    }
}