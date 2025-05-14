using CMPM.Utils;
using CMPM.Utils.RelicParsers;
using Newtonsoft.Json;
using UnityEngine;


namespace CMPM.Relics {
    public static class RelicBuilder {
        static RelicBuilder() {
            ParseRelicsJson(Resources.Load<TextAsset>("relics"));
        }

        static void ParseRelicsJson(TextAsset relicsJson) { }
    }

    #region JSON Parsing
    [JsonConverter(typeof(RelicTriggerTypeParser))]
    public enum TriggerType {
        TakeDamage,
        StandStill,
        OnKill
    }

    [JsonConverter(typeof(RelicEffectTypeParser))]
    public enum EffectType {
        GainMana,
        GainSpellpower
    }

    [JsonConverter(typeof(RelicEffectConditionParser))]
    public enum EffectCondition {
        Count,
        Move,
        CastSpell
    }

    [JsonConverter(typeof(RelicDataParser))]
    public readonly struct RelicData {
        public readonly string Name;
        public readonly int SpriteIndex;
        public readonly RelicTriggerData Trigger;
        public readonly RelicEffectData Effect;

        public RelicData(string name, int sprite, RelicTriggerData trigger, RelicEffectData effect) {
            Name        = name;
            SpriteIndex = sprite;
            Trigger     = trigger;
            Effect      = effect;
        }
    };

    public readonly struct RelicTriggerData {
        public readonly string Description;
        public readonly TriggerType Type;

        public RelicTriggerData(string description, TriggerType type) {
            Description = description;
            Type        = type;
        }
    }

    public readonly struct RelicEffectData {
        public readonly string Description;
        public readonly EffectType Type;
        public readonly RPNString Amount;
        public readonly EffectCondition Condition;

        public RelicEffectData(string description, EffectType type, RPNString amount, EffectCondition? condition) {
            Description = description;
            Type        = type;
            Amount      = amount;
            Condition   = condition ?? EffectCondition.Count;
        }
    }
    #endregion
}