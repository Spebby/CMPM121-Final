using System;
using CMPM.Core;
using CMPM.Spells;
using CMPM.Utils;
using CMPM.Utils.RelicParsers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace CMPM.Relics {
    public static class RelicBuilder {
        static RelicBuilder() {
            ParseRelicsJson(Resources.Load<TextAsset>("relics"));
        }

        static void ParseRelicsJson(TextAsset relicsJson) {
            foreach (JProperty _ in JObject.Parse(relicsJson.text).Properties()) {
                RelicData d = _.Value.ToObject<RelicData>();
                RelicRegistry.Register(d.Name.GetHashCode(), new Relic(GameManager.Instance.PlayerController, d));
            }
        }
    }

    public class Relic {
        #region Privates
        readonly RelicData _data;
        readonly RelicEffect _effect;
        #endregion

        public Relic(PlayerController player, RelicData data) {
            SpellCaster caster = player;
            _data  = data;
            
            RelicData.RelicEffectData effect = data.Effect;
            _effect = effect.Type switch {
                EffectType.GainMana       => new GainManaEffect(caster, effect.Amount),
                EffectType.GainSpellpower => new GainSpellpowerEffect(caster, effect.Amount),
                EffectType.RandomBoost    => new GainRandomBuff(player, effect.Amount),
                _                         => throw new NotImplementedException($"Effect type {effect.Type} is not implemented")
            };
            
            // todo: this is all well and good, but I want to support more complicated pre-conditions
            // Would be good to speak with others about this.
            RelicData.RelicPreconditionData precondition = _data.Precondition;
            RelicTrigger                    trigger      = null;
            switch (precondition.Type) {
                case PreconditionType.TakeDamage:
                    EventBus.Instance.OnDamage += (_, _, h) => {
                        if (h.Owner != GameManager.Instance.Player) return;
                        OnCallback();
                    };
                    break;
                case PreconditionType.StandStill:
                    trigger = new RelicStandstillTrigger(_effect, precondition.Amount ?? new RPNString("0.5"));
                    EventBus.Instance.OnPlayerStandstill += trigger.OnTrigger;
                    break;
                case PreconditionType.OnKill:
                    EventBus.Instance.OnEnemyDeath += _ => OnCallback();
                    break;
                case PreconditionType.Timer:
                    /* I'm making the executive decision that if the effect type is random boost, we want to overwrite
                     * the previous random boost. There are a lot of cases where timers shouldn't mess w/ previous
                     * triggers but in this case it seems warranted.
                     *
                     * This should really be handled by the expiration switch but im a bit too tired to think it through rn
                     */
                    bool shouldOverwrite = effect.Type == EffectType.RandomBoost;
                    trigger = new RelicTimerTrigger(_effect, _data.Precondition.Range ?? throw new Exception($"{Name} must define a range for precondition {precondition.Type}!"), shouldOverwrite);
                    trigger.OnTrigger();
                    break;
                default:
                    throw new NotImplementedException($"Precondition type {precondition.Type} is not implemented");
            }

            RelicExpire expire;
            switch (effect.Expiration) {
                case EffectExpiration.Move:
                    EventBus.Instance.OnPlayerMove += f => {
                        if (f > Mathf.Epsilon) return;
                        OnCancel();
                    };
                    break;
                case EffectExpiration.CastSpell:
                    caster.OnCast += OnCancel;
                    break;
                case EffectExpiration.Timer:
                    if (trigger is RelicTimerTrigger timerTrigger) {
                        timerTrigger.OnTriggered += _effect.RevertEffect;
                        break;
                    }
                    
                    expire = new RelicTimerExpire(_effect, effect.Range ?? throw new Exception($"{Name} must define a range for effect expiration {effect.Expiration}!"));
                    expire.OnTrigger();
                    break;
                case EffectExpiration.None:
                    break;
                default:
                    throw new NotImplementedException($"Effect cancellation condition {effect.Expiration} is not implemented");
            }
        }

        #region APIs
        public string Name => _data.Name;
        public PreconditionType PreconditionType => _data.Precondition.Type;
        public string PreconditionDescription => _data.Precondition.Description;
        public EffectType EffectType => _data.Effect.Type;
        public EffectExpiration EffectExpiration => _data.Effect.Expiration;
        public string EffectDescription => _data.Effect.Description;
        public new int GetHashCode => Name.GetHashCode();
        public static implicit operator RelicData(Relic relic) => relic._data;
        #endregion
        
        void OnCallback() {
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
        OnKill,
        Timer
    }

    [JsonConverter(typeof(RelicEffectTypeParser))]
    public enum EffectType {
        GainMana,
        GainSpellpower,
        RandomBoost
    }

    [JsonConverter(typeof(RelicEffectConditionParser))]
    public enum EffectExpiration {
        None,
        Move,
        Timer,
        CastSpell
    }

    
    [JsonConverter(typeof(RelicRangeParser))]
    public readonly struct RPNRange {
        public readonly RPNString Min;
        public readonly RPNString Max;

        public RPNRange(RPNString min, RPNString max) {
            Min = min;
            Max = max;
        }
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
            public readonly RPNRange? Range;
            
            public RelicPreconditionData(string description, PreconditionType type, RPNString? amount, RPNRange? range) {
                Description = description;
                Type        = type;
                Amount      = amount;
                Range       = range;
            }
        }
        public readonly struct RelicEffectData {
            public readonly string Description;
            public readonly EffectType Type;
            public readonly RPNString Amount;
            public readonly EffectExpiration Expiration;
            public readonly RPNRange? Range;

            public RelicEffectData(string description, EffectType type, RPNString amount, EffectExpiration? condition, RPNRange? range) {
                Description = description;
                Type        = type;
                Amount      = amount;
                Expiration  = condition ?? EffectExpiration.None;
                Range       = range;
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
