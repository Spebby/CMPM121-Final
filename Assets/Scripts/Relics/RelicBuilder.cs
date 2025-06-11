using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CMPM.Core;
using CMPM.Relics.Effects;
using CMPM.Relics.Expires;
using CMPM.Relics.Triggers;
using CMPM.Spells;
using CMPM.Spells.Modifiers;
using CMPM.Status;
using CMPM.Utils.RelicParsers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using RPNString = CMPM.Utils.RPNString;
using CMPM.DamageSystem;


namespace CMPM.Relics {
    public static class RelicBuilder {
        static bool _initialised;
        
        static RelicBuilder() {
            TextAsset json = Resources.Load<TextAsset>("relics");
            foreach (JToken token in JArray.Parse(json.text)) {
                RelicRegistry.Register(token.ToObject<RelicData>());
            }
        }
        
        public static void Initialise() {
            if (_initialised) return;
            _initialised = true;
        }

        public static RelicData[] GetRelicSet(in BitArray flag, int count, out BitArray updatedFlag) {
            if (count == 0) {
                updatedFlag = null;
                return null;
            }

            updatedFlag = flag.Clone() as BitArray;
            List<RelicData> relics = new();
            for (int i = 0; i < count; i++) {
                int index = RelicRegistry.GetRandomUnique(updatedFlag, out RelicData? relic);
                if (index == -1) break;
                updatedFlag![index] = true;
                relics.Add(relic!.Value);
            }

            return relics.ToArray(); 
        }
        
        public static Relic CreateRelic(in RelicData relic) => new (GameManager.Instance.PlayerController, relic);
        
        public static Relic[] CreateRelics(in BitArray flag, int count, out BitArray updatedFlag) {
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
                relics.Add(CreateRelic(relic!.Value));
            }

            return relics.ToArray();
        }

        public static Relic[] CreateRelics(in RelicData[] relics) {
            Relic[] relicArray = new Relic[relics.Length];
            for (int i = 0; i < relics.Length; i++) {
                relicArray[i] = CreateRelic(relics[i]);
            }
            return relicArray;
        }
    }

    public class Relic {
        #region Privates
        readonly RelicData _data;
        readonly IRelicEffect[] _effects;
        public bool IsActive { get; private set; }
        #endregion

        public Relic(in PlayerController player, in RelicData data) {
            SpellCaster caster = player;
            _data  = data;


            RelicData.RelicPreconditionData precondition = _data.Precondition;
            RelicTrigger trigger = precondition.Type switch {
                PreconditionType.TakeDamage => null,
                PreconditionType.StandStill => new RelicStandstill(this, precondition.Amount ?? new RPNString("0.5")),
                PreconditionType.Timer => new RelicTimer(this,
                                                         precondition.Range ??
                                                         throw new Exception(
                                                             $"{Name} must define a range for precondition {precondition.Type}!")),
                PreconditionType.OnKill    => new RelicKillCondition(this, precondition.Amount ?? new RPNString("1")),
                PreconditionType.OnHit     => null,
                PreconditionType.None      => null,
                PreconditionType.WaveStart => null,
                PreconditionType.WaveEnd   => null,
                _                          => throw new NotImplementedException($"Precondition type {precondition.Type} is not implemented")
            };
            
            // Wire the trigger event
            switch (precondition.Type) {
                case PreconditionType.TakeDamage:
                    EventBus.Instance.OnDamage += (_, _, h) => {
                        if (h.Owner != GameManager.Instance.PlayerController) return;
                        OnActivate();
                    };
                    ShouldHighlight = false;
                    break;
                case PreconditionType.StandStill:
                    EventBus.Instance.OnPlayerStandstill += () => trigger!.OnTrigger();
                    // This is kind of bad, so I'd prefer a way of doing this that's cleaner.
                    ShouldHighlight = true;
                    break;
                case PreconditionType.OnKill:
                    EventBus.Instance.OnEnemyDeath += _ => trigger!.OnTrigger();
                    ShouldHighlight = false;
                    break;
                case PreconditionType.OnHit:
                    Hittable.Team friendly = player.Team;
                    EventBus.Instance.OnDamage += (_, _, h) => {
                        if (h.team == friendly) return;
                        OnActivate();
                    };
                    ShouldHighlight = false;
                    break;
                case PreconditionType.Timer:
                    /* I'm making the executive decision that if the effect type is random boost, we want to overwrite
                     * the previous random boost. There are a lot of cases where timers shouldn't mess w/ previous
                     * triggers but in this case it seems warranted.
                     *
                     * This should really be handled by the expiration switch but im a bit too tired to think it through rn
                     */
                    RelicTimer t = trigger as RelicTimer;
                    t!.OnTrigger();
                    ShouldHighlight =  true;
                    break;
                case PreconditionType.None:
                    // handled at end of function
                    ShouldHighlight = false;
                    break;
                case PreconditionType.WaveEnd:
                    EventBus.Instance.OnWaveEnd += OnActivate;
                    ShouldHighlight             =  false;
                    break;
                case PreconditionType.WaveStart:
                    EventBus.Instance.OnWaveStart += OnActivate;
                    ShouldHighlight               =  false;
                    break;
                default:
                    throw new NotImplementedException($"Precondition type {precondition.Type} is not implemented");
            }
            
            
            List<IRelicEffect> effectsList = new();
            foreach (RelicData.RelicEffectData effect in data.Effects) {
                int[] modifiers = null;
                switch (effect.Type) {
                    case EffectType.ModifySpellCooldownP: {
                        SpellStatModifier m    = new(null, null, null, new RPNString($"value {effect.Amount.ToString()} *"));
                        int               hash = m.GetHashCode();
                        SpellModifierRegistry.Register(hash, m);
                        modifiers = new[] { hash };
                        break;
                    }
                    case EffectType.ModifySpellCostP: {
                        SpellStatModifier m    = new(null, new RPNString($"value {effect.Amount.ToString()} *"));
                        int               hash = m.GetHashCode();
                        SpellModifierRegistry.Register(hash, m);
                        modifiers = new[] { hash };
                        break;
                    }
                    case EffectType.StatusBurn: {
                        RPNRange range        = effect.Range ?? throw new NullReferenceException($"{effect.Type} requires an effect range timer!");
                        SpellStatusModifier m = new((entity) => new DOTStatus(entity, range.Max, effect.Amount, 0.4f, Damage.Type.FIRE));
                        int hash = m.GetHashCode();
                        SpellModifierRegistry.Register(hash, m);
                        modifiers = new[] { hash };
                        break;
                    }
                    case EffectType.GainMana:
                    case EffectType.GainSpellpower:
                    case EffectType.GainHealth:
                    case EffectType.GainMaxHealth:
                    case EffectType.GainSpeed:
                    case EffectType.RandomBoost:
                    default:
                        break;
                }

                effectsList.Add(effect.Type switch {
                    EffectType.GainMana             => new GainStatEffect(effect.Amount, caster.AddMana),
                    EffectType.GainSpellpower       => new GainStatEffect(effect.Amount, caster.AddSpellpower),
                    EffectType.GainHealth           => new GainStatEffect(effect.Amount, player.HP.Heal),
                    EffectType.GainMaxHealth        => new GainStatEffect(effect.Amount, player.HP.AddHPCap),
                    EffectType.GainSpeed            => new GainStatEffect(effect.Amount, player.ModifySpeed),
                    EffectType.RandomBoost          => new GainRandomBuff(player, effect.Amount),
                    EffectType.ModifySpellCooldownP => new BaseSpellModifierEffect(player, modifiers),
                    EffectType.ModifySpellCostP     => new BaseSpellModifierEffect(player, modifiers),
                    EffectType.StatusBurn           => new BaseSpellModifierEffect(player, modifiers),
                    _                               => throw new NotImplementedException($"Effect type {effect.Type} is not implemented")
                });

                RelicExpire expire;
                switch (effect.Expiration) {
                    case EffectExpiration.Move:
                        EventBus.Instance.OnPlayerMove += f => {
                            if (f <= Mathf.Epsilon) return;
                            OnDeactivate();
                        };
                        break;
                    case EffectExpiration.CastSpell:
                        caster.OnCast += OnDeactivate;
                        break;
                    case EffectExpiration.Timer:
                        expire = new RelicTimerExpire(
                            this, effect.Range ?? throw new Exception($"{Name} must define a range for effect expiration {effect.Expiration}!"));

                        if (trigger is RelicTimer timerTrigger) {
                            timerTrigger.OnTriggered += () => expire.OnTrigger();
                            break;
                        }

                        expire.OnTrigger();
                        break;
                    case EffectExpiration.Overwrite:
                        ShouldHighlight = false;
                        break;
                    case EffectExpiration.None:
                        // Nothing to wire
                        break;

                    default:
                        throw new NotImplementedException($"Effect cancellation condition {effect.Expiration} is not implemented");
                } 
            }
            _effects = effectsList.ToArray();

            // This is a passive relic, which means its effects are *always* active.
            if (precondition.Type == PreconditionType.None) {
                OnActivate();
            }
        }

        #region APIs
        public string Name => _data.Name;
        public string Description => _data.GetFullDescription();
        public uint Sprite => _data.Sprite;
        public new int GetHashCode => Name.GetHashCode();
        public static implicit operator RelicData(Relic relic) => relic._data;
        public bool ShouldHighlight { get; private set; }
        #endregion
        
        internal void OnActivate() {
            for (int i = 0; i < _effects.Length; i++) {
                IRelicEffect              e = _effects[i];
                RelicData.RelicEffectData d = _data.Effects[i];

                if (d.Expiration == EffectExpiration.Overwrite && IsActive) {
                    e.RevertEffect();
                }
                e.ApplyEffect();
            }
            IsActive = true;
        }

        internal void OnDeactivate() {
            IsActive = false;
            foreach (IRelicEffect e in _effects) {
                e.RevertEffect();
            }
        }
    }

    #region JSON Parsing
    [JsonConverter(typeof(RelicTriggerTypeParser))]
    public enum PreconditionType {
        TakeDamage,
        StandStill,
        OnKill,
        OnHit,
        Timer,
        WaveStart,
        WaveEnd,
		None
    }

    [JsonConverter(typeof(RelicEffectTypeParser))]
    public enum EffectType {
        GainMana,
        GainSpellpower,
		GainHealth,
        GainMaxHealth,
        GainSpeed,
        RandomBoost,
		ModifySpellCooldownP,
		ModifySpellCostP,
        StatusBurn
    }

    [JsonConverter(typeof(RelicEffectConditionParser))]
    public enum EffectExpiration {
        None,
        Move,
        Timer,
        Overwrite,
        CastSpell
    }

    
    [JsonConverter(typeof(RelicRangeParser))]
    // ReSharper disable once InconsistentNaming
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
        public readonly uint   Sprite;
        public readonly string Rarity;

        public readonly RelicPreconditionData Precondition;
        public readonly RelicEffectData[] Effects;

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

        public RelicData(string name, string description, uint sprite, string rarity, RelicPreconditionData precondition,
                         RelicEffectData[] effects) {
            Name         = name;
            Description  = description;
            Sprite       = sprite;
            Rarity       = rarity;
            Precondition = precondition;
            Effects      = effects;
            
        }

        public string GetFullDescription() {
            bool hasPrecondition = !string.IsNullOrEmpty(Precondition.Description);
            bool hasEffect       = !Effects.Any(v => string.IsNullOrEmpty(v.Description));
            string str = $"{Description}\n{(hasEffect || hasPrecondition ? '\n' : "")}{(hasPrecondition ? $"{Precondition.Description}\n" : "")}";
            if (!hasEffect) return str;
            str = Effects.Aggregate(str, (current, effect) => current + $"{effect.Description}\n");
            return str;
        }

        #region Equality Helpers
        public bool Equals(RelicData other) => Name == other.Name && Description == other.Description && Sprite == other.Sprite && Precondition.Equals(other.Precondition) && Equals(Effects, other.Effects);
        public override bool Equals(object obj) => obj is RelicData other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Name, Description, Sprite, Precondition, Effects);
        #endregion
    }
    #endregion
}