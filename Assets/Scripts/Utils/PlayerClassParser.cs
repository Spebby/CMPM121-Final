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

                string description = data["description"]?.Value<string>() ?? throw new JsonSerializationException($"Class {prop.Name} is missing description!");
                uint spriteIndex   = data["sprite"]?.Value<uint>()        ?? throw new JsonSerializationException($"Class {prop.Name} is missing sprite!");

                RPNString health     = data["health"]?.ToObject<RPNString>() ?? throw new JsonSerializationException($"Class {prop.Name} is missing health");
                RPNString mana       = data["mana"]?.ToObject<RPNString>()   ?? throw new JsonSerializationException($"Class {prop.Name} is missing mana");
                RPNString manaRegen  = data["mana_regeneration"]?.ToObject<RPNString>() ?? throw new JsonSerializationException($"Class {prop.Name} is missing mana_regeneration");
                RPNString spellpower = data["spellpower"]?.ToObject<RPNString>() ?? throw new JsonSerializationException($"Class {prop.Name} is missing spellpower");
                RPNString speed      = data["speed"]?.ToObject<RPNString>()      ?? throw new JsonSerializationException($"Class {prop.Name} is missing speed");

                PlayerClass playerClass = new(type, description, spriteIndex, health, mana, manaRegen, spellpower, speed);
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