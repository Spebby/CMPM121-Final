using System;
using CMPM.Relics;
using Newtonsoft.Json;


namespace CMPM.Utils.RelicParsers {
    public class RelicDataParser : JsonConverter<RelicData> {
        public override void WriteJson(JsonWriter writer, RelicData value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override RelicData ReadJson(JsonReader reader, Type objectType, RelicData existingValue, bool hasExistingValue,
                                             JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}