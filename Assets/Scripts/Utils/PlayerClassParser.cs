using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerClass = CMPM.Core.PlayerController.PlayerClass;


namespace CMPM.Utils {
    public sealed class PlayerClassMapParser : JsonConverter<Dictionary<PlayerClass.Type, PlayerClass>> {
        public override Dictionary<PlayerClass.Type, PlayerClass> ReadJson(
            JsonReader reader, Type objectType, Dictionary<PlayerClass.Type, PlayerClass> existingValue,
            bool hasExistingValue, JsonSerializer serializer) {
            JObject                                   root   = JObject.Load(reader);
            Dictionary<PlayerClass.Type, PlayerClass> result = new();

            foreach (JProperty prop in root.Properties()) {
                PlayerClass.Type type = JsonConvert.DeserializeObject<PlayerClass.Type>(
                    $"\"{prop.Name}\"", new PlayerClassTypeParser());

                JObject data = (JObject)prop.Value;

                uint spriteIndex = data["sprite"]?.Value<uint>() ?? 0;

                RPNString health     = new(data["health"]?.Value<string>() ?? "0");
                RPNString mana       = new(data["mana"]?.Value<string>() ?? "0");
                RPNString manaRegen  = new(data["mana_regeneration"]?.Value<string>() ?? "0");
                RPNString spellpower = new(data["spellpower"]?.Value<string>() ?? "0");
                RPNString speed      = new(data["speed"]?.Value<string>() ?? "0");

                PlayerClass playerClass = new(type, spriteIndex, health, mana, manaRegen, spellpower, speed);
                result[type] = playerClass;
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<PlayerClass.Type, PlayerClass> value,
                                       JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    } 
}