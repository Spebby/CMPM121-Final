using System;
using CMPM.DamageSystem;
using CMPM.Spells;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils {
    public class SpellDamageParser : JsonConverter<SpellDamageData> {
        public override SpellDamageData ReadJson(JsonReader reader, Type objectType, SpellDamageData existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            string amountStr = obj["amount"]?.ToString();
            string typeStr   = obj["type"]?.ToString();

            if (amountStr == null || typeStr == null)
                throw new JsonException("Missing 'amount' or 'type' field in damage JSON");

            RPNString   rpn     = new(amountStr);
            Damage.Type dmgType = Damage.TypeFromString(typeStr);

            return new SpellDamageData(rpn, dmgType);
        }

        public override void WriteJson(JsonWriter writer, SpellDamageData value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("amount");
            writer.WriteValue(value.DamageRPN.ToString());
            writer.WritePropertyName("type");
            writer.WriteValue(value.Type.ToString().ToLower());
            writer.WriteEndObject();
        }
    }
}