using System;
using CMPM.Relics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.RelicParsers {
    public class RelicRangeParser : JsonConverter<RPNRange> {
        public override void WriteJson(JsonWriter writer, RPNRange value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override RPNRange ReadJson(JsonReader reader, Type objectType, RPNRange existingValue,
                                                        bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            return new RPNRange(
                obj["min"]?.ToObject<RPNString>() ?? throw new JsonException("Missing precondition.min"),
                obj["max"]?.ToObject<RPNString>() ?? throw new JsonException("Missing precondition.max")
            );
        }
    }
}