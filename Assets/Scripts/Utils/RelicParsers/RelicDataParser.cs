using System;
using System.Linq;
using CMPM.Relics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.RelicParsers {
    public class RelicDataParser : JsonConverter<RelicData> {
        public override RelicData ReadJson(JsonReader reader, Type objectType, RelicData existingValue,
                                           bool hasExistingValue,
                                           JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            string name        = obj["name"]?.ToString()         ?? throw new JsonException("Missing name");
            string description = obj["description"]?.ToString()  ?? throw new JsonException("Missing description");
            uint   sprite      = obj["sprite"]?.ToObject<uint>() ?? throw new JsonException("Missing sprite");

            // Parse Precondition
            JObject pre = obj["precondition"]?.ToObject<JObject>() ?? throw new JsonException("Missing precondition");
            PreconditionType type = pre["type"]?.ToObject<PreconditionType>(serializer) ??
                                    throw new JsonException("Invalid precondition.type");
            RelicData.RelicPreconditionData relicPrecondition = new(
                pre["description"]?.ToString() ?? (type == PreconditionType.None ? "" : throw new JsonException("Missing precondition.description")),
                type,
                pre["amount"]?.ToObject<RPNString>(serializer),
                pre["range"]?.ToObject<RPNRange>(serializer)
            );

            // Parse Effect
            JArray effArray = obj["effect"]?.ToObject<JArray>() ?? throw new JsonException("Missing effect");
            RelicData.RelicEffectData[] effects = effArray
                                                 .Select(effToken => {
                                                      JObject eff = (JObject)effToken;
                                                      return new RelicData.RelicEffectData(
                                                          eff["description"]?.ToString() ??
                                                          throw new JsonException("Missing effect.description"),
                                                          eff["type"]?.ToObject<EffectType>(serializer) ??
                                                          throw new JsonException("Invalid effect.type"),
                                                          new RPNString(
                                                              eff["amount"]?.ToString() ??
                                                              throw new JsonException("Missing effect.amount")),
                                                          eff["until"]?.ToObject<EffectExpiration>(serializer),
                                                          eff["range"]?.ToObject<RPNRange>(serializer)
                                                      );
                                                  })
                                                 .ToArray();
            
            return new RelicData(name, description, sprite, relicPrecondition, effects);
        }
        
        public override void WriteJson(JsonWriter writer, RelicData value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}