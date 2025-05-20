using System;
using CMPM.Core;
using CMPM.Spells;
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

    public class Relic {
        public readonly SpellCaster Caster;

        #region Privates
        readonly RelicData _data;
        readonly RelicPrecondition _precondition;
        readonly RelicEffect _effect;
        #endregion

        public Relic(SpellCaster caster, RelicData data) {
            Caster = caster;
            _data  = data;

            RelicData.RelicPreconditionData precondition = _data.Precondition;
            switch (precondition.Type) {
                case PreconditionType.TakeDamage:
                    break;
                case PreconditionType.StandStill:
                    
                case PreconditionType.OnKill:
                default:
                    throw new NotImplementedException($"Precondition type {precondition.Type} is not implemented");
            }

            RelicData.RelicEffectData effect = data.Effect;
            _effect = effect.Type switch {
                EffectType.GainMana       => new GainManaEffect(caster, effect.Amount),
                EffectType.GainSpellpower => new GainSpellpowerEffect(caster, effect.Amount),
                _                         => throw new NotImplementedException($"Effect type {effect.Type} is not implemented")
            };

            switch (effect.Expiration) {
                case EffectExpiration.Move:
                    EventBus.Instance.OnPlayerMove += OnCancel;
                    break;
                case EffectExpiration.CastSpell:
                    Caster.OnCast += OnCancel;
                    break;
                case EffectExpiration.None:
                    break;
                default:
                    throw new NotImplementedException($"Effect cancellation condition {effect.Expiration} is not implemented");
            }
        }

        void OnCallback() {
            if (!_precondition.Evaluate()) return;
            _effect.ApplyEffect();
        }

        void OnCancel() {
            _effect.RevertEffect();
        }
    }

    #region JSON Parsing
    [JsonConverter(typeof(RelicTriggerTypeParser))]
    public enum PreconditionType {
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
    public enum EffectExpiration {
        None,
        Move,
        CastSpell
    }

    [JsonConverter(typeof(RelicDataParser))]
    public readonly struct RelicData {
        public readonly string Name;
        public readonly int SpriteIndex;

        public readonly RelicPreconditionData Precondition;
        public readonly RelicEffectData Effect;
        
        public readonly struct RelicPreconditionData {
            public readonly string Description;
            public readonly PreconditionType Type;
            public readonly RPNString? Amount;
            
            public RelicPreconditionData(string description, PreconditionType type, RPNString? amount) {
                Description = description;
                Type        = type;
                Amount      = amount;
            }
        }
        public readonly struct RelicEffectData {
            public readonly string Description;
            public readonly EffectType Type;
            public readonly RPNString Amount;
            public readonly EffectExpiration Expiration;

            public RelicEffectData(string description, EffectType type, RPNString amount, EffectExpiration? condition) {
                Description = description;
                Type        = type;
                Amount      = amount;
                Expiration  = condition ?? EffectExpiration.None;
            }
        }
        
        public RelicData(string name, int sprite, RelicPreconditionData precondition, RelicEffectData effect) {
            Name              = name;
            SpriteIndex       = sprite;
            Precondition = precondition;
            Effect = effect;
        }
    }
    #endregion
}
