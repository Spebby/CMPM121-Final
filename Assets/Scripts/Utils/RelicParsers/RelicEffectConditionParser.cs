using System;
using CMPM.Relics;
using Newtonsoft.Json;


namespace CMPM.Utils.RelicParsers {
    public class RelicEffectConditionParser : JsonConverter<EffectCondition> {
        public override EffectCondition ReadJson(JsonReader reader, Type objectType, EffectCondition existingValue,
                                                 bool hasExistingValue,
                                                 JsonSerializer serializer) {
            string str = reader.Value as string;
            if (string.IsNullOrEmpty(str))
                return EffectCondition.Count;

            return str.ToLower() switch {
                "cast-spell" => EffectCondition.CastSpell,
                "count"      => EffectCondition.Count,
                "move"       => EffectCondition.Move,
                _            => throw new NotImplementedException($"Unknown condition type '{str}'")
            };
        }

        public override void WriteJson(JsonWriter writer, EffectCondition value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}