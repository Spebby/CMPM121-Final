using System;
using System.Collections;
using System.Collections.Generic;
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
            TextAsset json = Resources.Load<TextAsset>("relics");
            foreach (JToken token in JArray.Parse(json.text)) {
                RelicRegistry.Register(token.ToObject<RelicData>());
            }
        }
        
        public static Relic[] CreateRelics(BitArray flag, int count, out BitArray updatedFlag) {
            if (count == 0) {
                updatedFlag = null;
                return null;
            }

            updatedFlag = flag.Clone() as BitArray;
            List<Relic> relics = new();
            for (int i = 0; i < count; i++) {
                int index = RelicRegistry.GetRandomUnique(updatedFlag, out RelicData? relic);
                if (index == -1) break;
                updatedFlag![index] = true;
                relics.Add(new Relic(GameManager.Instance.PlayerController, relic!.Value));
            }

            return relics.ToArray();
        }

        static void ParseRelicsJson(TextAsset relicsJson) {

        }
    }

    public class Relic {
        #region Privates
        readonly RelicData _data;
        readonly RelicEffect _effect;
        public bool IsActive { get; private set; }
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
                        OnActivate();
                    };
                    break;
                case PreconditionType.StandStill:
                    trigger = new RelicStandstillTrigger(_effect, precondition.Amount ?? new RPNString("0.5"));
                    EventBus.Instance.OnPlayerStandstill += () => trigger.OnTrigger(() => IsActive = true);
                    // This is kind of bad, so I'd prefer a way of doing this that's cleaner.
                    break;
                case PreconditionType.OnKill:
                    EventBus.Instance.OnEnemyDeath += _ => OnActivate();
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
                    trigger.OnTrigger(() => IsActive = true);
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
                        timerTrigger.OnTriggered += OnCancel;
                        break;
                    }

                    expire = new RelicTimerExpire(
                        _effect,
                        effect.Range ??
                        throw new Exception($"{Name} must define a range for effect expiration {effect.Expiration}!"));
                    expire.OnTrigger(() => IsActive = false);
                    break;
                case EffectExpiration.None:
                    break;
                default:
                    throw new NotImplementedException($"Effect cancellation condition {effect.Expiration} is not implemented");
            }
        }

        #region APIs
        public string Name => _data.Name;
        public string Description => _data.Description;
        public uint Sprite => _data.SpriteIndex;
        public PreconditionType PreconditionType => _data.Precondition.Type;
        public string PreconditionDescription => _data.Precondition.Description;
        public EffectType EffectType => _data.Effect.Type;
        public EffectExpiration EffectExpiration => _data.Effect.Expiration;
        public string EffectDescription => _data.Effect.Description;
        public new int GetHashCode => Name.GetHashCode();
        public static implicit operator RelicData(Relic relic) => relic._data;
        public RelicData Data => _data;
        #endregion
        
        void OnActivate() {
            _effect.ApplyEffect();
            IsActive = true;
        }

        void OnCancel() {
            _effect.RevertEffect();
            IsActive = false;
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
    public readonly struct RPNRange : IEquatable<RPNRange> {
        public readonly RPNString Min;
        public readonly RPNString Max;

        public RPNRange(RPNString min, RPNString max) {
            Min = min;
            Max = max;
        }

        #region Equality Helpers
        public bool Equals(RPNRange other) => Min.Equals(other.Min) && Max.Equals(other.Max);
        public override bool Equals(object obj) => obj is RPNRange other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Min, Max);
        #endregion
    }
    
    [JsonConverter(typeof(RelicDataParser))]
    public readonly struct RelicData : IEquatable<RelicData> {
        public readonly string Name;
        public readonly string Description;
        public readonly uint SpriteIndex;

        public readonly RelicPreconditionData Precondition;
        public readonly RelicEffectData Effect;

        public readonly struct RelicPreconditionData : IEquatable<RelicPreconditionData> {
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

            #region Equality Helpers
            public bool Equals(RelicPreconditionData other) => Description == other.Description && Type == other.Type && Nullable.Equals(Amount, other.Amount) && Nullable.Equals(Range, other.Range);
            public override bool Equals(object obj) => obj is RelicPreconditionData other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Description, (int)Type, Amount, Range);
            #endregion
        }
        public readonly struct RelicEffectData : IEquatable<RelicEffectData> {
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

            #region Equality Helpers
            public bool Equals(RelicEffectData other) => Description == other.Description && Type == other.Type && Amount.Equals(other.Amount) && Expiration == other.Expiration && Nullable.Equals(Range, other.Range);
            public override bool Equals(object obj) => obj is RelicEffectData other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Description, (int)Type, Amount, (int)Expiration, Range);
            #endregion
        }

        public RelicData(string name, string description, uint sprite, RelicPreconditionData precondition,
                         RelicEffectData effect) {
            Name         = name;
            Description  = description;
            SpriteIndex  = sprite;
            Precondition = precondition;
            Effect       = effect;
        }

        #region Equality Helpers
        public bool Equals(RelicData other) => Name == other.Name && Description == other.Description && SpriteIndex == other.SpriteIndex && Precondition.Equals(other.Precondition) && Effect.Equals(other.Effect);
        public override bool Equals(object obj) => obj is RelicData other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Name, Description, SpriteIndex, Precondition, Effect);
        #endregion
    }
    #endregion
}
