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
                "gain-health"            => EffectType.GainHealth,
                "gain-speed"             => EffectType.GainSpeed,
                "gain-max-health"        => EffectType.GainMaxHealth,
                "random-boost"           => EffectType.RandomBoost,
                "modify-spell-cooldown%" => EffectType.ModifySpellCooldownP,
                "modify-spell-cost%"     => EffectType.ModifySpellCostP,
                "status-burn"            => EffectType.StatusBurn,
                _                        => throw new NotImplementedException($"Unknown effect type '{str}'")
            };
        }

        public override void WriteJson(JsonWriter writer, EffectType value, JsonSerializer serializer) {
            string str = value switch {
                EffectType.GainMana             => "gain-mana",
                EffectType.GainSpellpower       => "gain-spellpower",
                EffectType.GainHealth           => "gain-health",
                EffectType.GainMaxHealth        => "gain-max-health",
                EffectType.GainSpeed            => "gain-speed",
                EffectType.RandomBoost          => "random-boost",
                EffectType.ModifySpellCooldownP => "modify-spell-cooldown%",
                EffectType.ModifySpellCostP     => "modify-spell-cost%",
                EffectType.StatusBurn           => "status-burn",
                _                               => throw new NotImplementedException($"Unknown effect type '{value}'")
            };

            writer.WriteValue(str);
        }
    }
}
