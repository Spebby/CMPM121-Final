using System;
using CMPM.DamageSystem;
using Newtonsoft.Json;


namespace CMPM.Utils {
    public class DamageTypeParser : JsonConverter<Damage.Type> {
        public override Damage.Type ReadJson(JsonReader reader, Type objectType, Damage.Type existingValue,
                                             bool hasExistingValue, JsonSerializer serializer) {
            string str = reader.Value as string;
            if (string.IsNullOrEmpty(str)) {
                throw new ArgumentException("Damage Type cannot be null or empty");
            }

            return Damage.TypeFromString(str);
        }

        public override void WriteJson(JsonWriter writer, Damage.Type value, JsonSerializer serializer) {
            writer.WriteValue(value);
        }
    }
}