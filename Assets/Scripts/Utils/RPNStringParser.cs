using System;
using Newtonsoft.Json;


namespace CMPM.Utils {
    public class RPNStringParser : JsonConverter<RPNString> {
        public override RPNString ReadJson(JsonReader reader, Type objectType, RPNString existingValue,
                                           bool hasExistingValue, JsonSerializer serializer) {
            string str = reader.Value as string;
            if (string.IsNullOrEmpty(str)) {
                throw new ArgumentException("RPNString cannot be null or empty");
            }

            return new RPNString(str);
        }

        public override void WriteJson(JsonWriter writer, RPNString value, JsonSerializer serializer) {
            writer.WriteValue(value.String);
        }
    }
}