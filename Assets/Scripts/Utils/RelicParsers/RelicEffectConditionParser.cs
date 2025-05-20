using System;
using CMPM.Relics;
using Newtonsoft.Json;


namespace CMPM.Utils.RelicParsers {
    public class RelicEffectConditionParser : JsonConverter<EffectExpiration> {
        public override EffectExpiration ReadJson(JsonReader reader, Type objectType, EffectExpiration existingValue,
                                                 bool hasExistingValue,
                                                 JsonSerializer serializer) {
            string str = reader.Value as string;
            if (string.IsNullOrEmpty(str))
                return EffectExpiration.None;

            return str.ToLower() switch {
                "cast-spell" => EffectExpiration.CastSpell,
                "move"       => EffectExpiration.Move,
                "none"       => EffectExpiration.None,
                _            => throw new NotImplementedException($"Unknown condition type '{str}'")
            };
        }

        public override void WriteJson(JsonWriter writer, EffectExpiration value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}