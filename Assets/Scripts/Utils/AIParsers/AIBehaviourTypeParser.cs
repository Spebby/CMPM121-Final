using System;
using CMPM.AI;
using Newtonsoft.Json;


namespace CMPM.Utils.AIParsers {
    public class AIBehaviourTypeParser : JsonConverter<BehaviourType> {
        public override BehaviourType ReadJson(JsonReader reader, Type objectType, BehaviourType existingValue,
                                            bool hasExistingValue,
                                            JsonSerializer serializer) {
            string str = reader.Value as string;
            if (string.IsNullOrEmpty(str))
                throw new JsonReaderException("RelicEffectTypeParser expected string 'Effect.Type'!");

            return str.ToLower() switch {
                "support" => BehaviourType.Support,
                "swarmer" => BehaviourType.Swarmer,
                _         => throw new NotImplementedException($"Unknown AI behaviour type '{str}'")
            };
        }

        public override void WriteJson(JsonWriter writer, BehaviourType value, JsonSerializer serializer) {
            string str = value switch {
                BehaviourType.Swarmer => "swarmer",
                BehaviourType.Support => "support",
                _                     => throw new NotImplementedException($"Unknown effect type '{value}'")
            };

            writer.WriteValue(str);
        }
    }
}