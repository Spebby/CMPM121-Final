using System;
using CMPM.Core;
using Newtonsoft.Json;


namespace CMPM.Utils {
    public class PlayerClassTypeParser : JsonConverter<PlayerController.PlayerClass.Type> {
        public override PlayerController.PlayerClass.Type ReadJson(
            JsonReader reader, Type objectType, PlayerController.PlayerClass.Type existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) {
            string str = reader.Value as string;
            if (string.IsNullOrEmpty(str))
                throw new JsonReaderException("RelicEffectTypeParser expected string 'Effect.Type'!");

            return str.ToLower() switch {
                "mage"       => PlayerController.PlayerClass.Type.Mage,
                "battlemage" => PlayerController.PlayerClass.Type.Battlemage,
                "warlock"    => PlayerController.PlayerClass.Type.Warlock,
                _            => throw new NotImplementedException($"Unknown AI behaviour type '{str}'")
            };
        }

        public override void WriteJson(JsonWriter writer, PlayerController.PlayerClass.Type value,
                                       JsonSerializer serializer) {
            string str = value switch {
                PlayerController.PlayerClass.Type.Mage => "mage",
                PlayerController.PlayerClass.Type.Battlemage => "battlemage",
                PlayerController.PlayerClass.Type.Warlock => "warlock",
                _ => throw new NotImplementedException($"Unknown effect type '{value}'")
            };

            writer.WriteValue(str);
        }
    }
}