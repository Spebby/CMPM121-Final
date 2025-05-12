using System;
using CMPM.Relics;
using Newtonsoft.Json;


namespace CMPM.Utils.RelicParsers {
    public class RelicTriggerTypeParser : JsonConverter<TriggerType> {
        public override TriggerType ReadJson(JsonReader reader, Type objectType, TriggerType existingValue, bool hasExistingValue,
                                             JsonSerializer serializer) {
            string str = (reader.Value as string)?.ToLower();
            if (string.IsNullOrEmpty(str))
                throw new JsonReaderException("RelicTriggerTypeParser expected string 'Trigger.Type'!");

            return str switch {
                "on-kill"     => TriggerType.OnKill,
                "stand-still" => TriggerType.StandStill,
                "take-damage" => TriggerType.TakeDamage,
                _             => throw new NotImplementedException($"Unknown trigger type '{str}'")
            };
        }
        
        public override void WriteJson(JsonWriter writer, TriggerType value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}