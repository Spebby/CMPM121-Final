using System;
using CMPM.Relics;
using Newtonsoft.Json;


namespace CMPM.Utils.RelicParsers {
    public class RelicEffectTypeParser : JsonConverter<EffectType> {
        public override EffectType ReadJson(JsonReader reader, Type objectType, EffectType existingValue,
                                            bool hasExistingValue,
                                            JsonSerializer serializer) {
            string str = reader.Value as string;
            if (string.IsNullOrEmpty(str))
                throw new JsonReaderException("RelicEffectTypeParser expected string 'Effect.Type'!");

            return str.ToLower() switch {
                "gain-mana"              => EffectType.GainMana,
                "gain-spellpower"        => EffectType.GainSpellpower,
                "random-boost"           => EffectType.RandomBoost,
                "modify-spell-cooldown%" => EffectType.ModifySpellCooldownP,
                "modify-spell-cost%"     => EffectType.ModifySpellCostP,
                _                        => throw new NotImplementedException($"Unknown effect type '{str}'")
            };
        }

        public override void WriteJson(JsonWriter writer, EffectType value, JsonSerializer serializer) {
            string str = value switch {
                EffectType.GainMana             => "gain-mana",
                EffectType.GainSpellpower       => "gain-spellpower",
                EffectType.RandomBoost          => "random-boost",
                EffectType.ModifySpellCooldownP => "modify-spell-cooldown%",
                EffectType.ModifySpellCostP     => "modify-spell-cost%",
                _                               => throw new NotImplementedException($"Unknown effect type '{value}'")
            };

            writer.WriteValue(str);
        }
    }
}
