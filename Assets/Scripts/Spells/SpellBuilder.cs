using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Newtonsoft.Json.Linq;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using CMPM.Utils.SpellParsers;
using Newtonsoft.Json;
using Random = UnityEngine.Random;


namespace CMPM.Spells {
    public static class SpellBuilder {
        static SpellBuilder() {
            ParseSpellsJson(Resources.Load<TextAsset>("spells"), Resources.Load<TextAsset>("spell_modifiers"));
        }

        static void ParseSpellsJson(TextAsset spellsJson, TextAsset modifiersJson) {
            // Spells
            foreach (JProperty _ in JObject.Parse(spellsJson.text).Properties()) {
                SpellData s = _.Value.ToObject<SpellData>();
                SpellRegistry.Register(s.Name.GetHashCode(), s);
            }

            // Build Modifiers Library
            foreach (JProperty _ in JObject.Parse(modifiersJson.text).Properties()) {
                SpellModifierData s = _.Value.ToObject<SpellModifierData>();
                ISpellModifier mod = _.Name switch {
                    "damage_amp" => new SpellStatModifier(s.DamageModifier, s.ManaCostModifier),
                    "speed_amp"  => new SpellStatModifier(null, null, s.SpeedModifier),
                    "forceful"   => new SpellStatModifier(s.DamageModifier),
                    "doubler" => new SpellRepeaterModifier(s.Count, s.Delay, null, s.ManaCostModifier, null,
                                                           s.CooldownModifier),
                    "splitter" => new SpellSplitModifier(s.Count, s.Angle, null, s.ManaCostModifier),
                    "chaos"    => new SpellProjectileModifier(ProjectileType.SPIRALING, s.DamageModifier),
                    "homing" => new SpellProjectileModifier(ProjectileType.HOMING, s.DamageModifier,
                                                            s.ManaCostModifier),
                    "waver" => new SpellProjectileModifier(ProjectileType.SINE, null, s.ManaCostModifier),
                    _       => throw new NotImplementedException($"{_.Name} is not implemented")
                };

                SpellModifierRegistry.Register(s.Name.GetHashCode(), mod);
                SpellModifierDataRegistry.Register(s.Name.GetHashCode(), s);
            }
        }

        public static Spell MakeRandomSpell(SpellCaster owner, int maxModifiers = 3) {
            return BuildSpell(SpellRegistry.GetRandomKey(), owner, maxModifiers);
        }

        public static Spell BuildSpell(string spellName, SpellCaster owner, int maxModifiers = 3) {
            return BuildSpell(spellName.GetHashCode(), owner, maxModifiers);
        }

        //to make the spell
        public static Spell BuildSpell(int hash, SpellCaster owner, int maxModifiers = 3) {
            SpellData data = SpellRegistry.Get(hash);
            string    name = data.Name;

            int[] modifiers = null;
            if (maxModifiers > 0) {
                modifiers = new int[Random.Range(0, maxModifiers)];
                for (int i = 0; i < modifiers.Length; ++i) {
                    modifiers[i] = SpellModifierRegistry.GetRandomKey();
                }
            }

            ProjectileData  projectile = data.Projectile;
            ProjectileData? secondary  = data.SecondaryProjectile;


            switch (data.Name) {
                case "Arcane Bolt":
                    return new ArcaneBolt(owner, data.Name, data.ManaCost, data.Damage.DamageRPN, data.Damage.Type,
                                          projectile.Speed, data.Cooldown, projectile.Lifetime,
                                          data.Icon, modifiers);
                case "Magic Missile":
                    return new MagicMissile(owner, name, data.ManaCost, data.Damage.DamageRPN, data.Damage.Type,
                                            projectile.Speed, data.Cooldown, projectile.Lifetime,
                                            data.Icon, modifiers);
                case "Arcane Blast": {
                    if (secondary == null) throw new Exception($"{name} has no secondary projectile");
                    if (data.Count == null) throw new Exception($"{name} has no count defined");
                    return new ArcaneBlast(owner, name, data.ManaCost, data.Damage.DamageRPN, data.Damage.Type,
                                           projectile.Speed, data.Cooldown, projectile.Lifetime,
                                           data.Count.Value, secondary.Value, data.Icon, modifiers);
                }
                case "Arcane Spray":
                    if (data.Count == null) throw new Exception($"{name} has no count defined");
                    if (data.Spray == null) throw new Exception($"{name} has no spray defined");
                    return new ArcaneSpray(owner, name, data.ManaCost, data.Damage.DamageRPN, data.Damage.Type,
                                           projectile.Speed, data.Cooldown, projectile.Lifetime,
                                           data.Icon, data.Count.Value, data.Spray.Value, modifiers);
                case "Mystic River":
                    if (data.Count == null) throw new Exception($"{name} has no count defined");
                    if (data.Spray == null) throw new Exception($"{name} has no spray defined");
                    return new MysticRiver(owner, name, data.ManaCost, data.Damage.DamageRPN, data.Damage.Type,
                                           projectile.Speed, data.Cooldown, projectile.Lifetime, data.Icon,
                                           data.Count.Value, data.Spray.Value, modifiers);
                //then it is a modifier spell, by recursively calling the BuildSpell method
                default: {
                    throw new Exception($"Unknown spell name: {data.Name}");
                }
            }
        }
    }

    #region JSON Parsing
    // TODO: At some point making these *all* RPN Strings may be beneficial simply for added flexibility would be cool.
    // Though that would be bad for performance lol
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [JsonConverter(typeof(SpellDataParser))]
    public struct SpellData {
        #region Metadata
        public string Name;
        public string Description;
        public uint Icon;
        #endregion

        #region Stats
        public SpellDamageData Damage;
        public RPNString ManaCost;
        public RPNString Cooldown;

        public ProjectileData Projectile;
        public ProjectileData? SecondaryProjectile;

        public RPNString? Count;
        public RPNString? Spray;
        #endregion
    }

    [JsonConverter(typeof(SpellDamageParser))]
    public struct SpellDamageData {
        public readonly RPNString DamageRPN;
        public readonly Damage.Type Type;

        public SpellDamageData(RPNString damageRPN, Damage.Type type) {
            DamageRPN = damageRPN;
            Type      = type;
        }
    }

    [JsonConverter(typeof(ProjectileDataParser))]
    public readonly struct ProjectileData {
        public readonly ProjectileType Trajectory;
        public readonly RPNString Speed;
        public readonly RPNString? Lifetime;
        public readonly int Sprite;

        public ProjectileData(ProjectileType trajectory, RPNString speed, int sprite, RPNString? lifetime) {
            Trajectory = trajectory;
            Speed      = speed;
            Lifetime   = lifetime;
            Sprite     = sprite;
        }
    }

    [JsonConverter(typeof(SpellModifierDataParser))]
    public readonly struct SpellModifierData {
        #region Metadata
        public readonly string Name;
        public readonly string Description;
        #endregion

        #region Stats
        public readonly RPNString DamageModifier;
        public readonly RPNString ManaCostModifier;
        public readonly RPNString SpeedModifier;
        public readonly RPNString CooldownModifier;
        public readonly RPNString LifetimeModifier;

        public readonly ProjectileType? Type;
        public readonly RPNString Angle;
        public readonly RPNString Delay;
        public readonly RPNString Count;

        public SpellModifierData(string name, string description, RPNString damageModifier, RPNString manaCostModifier,
                                 RPNString speedModifier, RPNString cooldownModifier, RPNString lifetimeModifier,
                                 ProjectileType? type, RPNString angle, RPNString delay, RPNString count) {
            Name             = name;
            Description      = description;
            DamageModifier   = damageModifier;
            ManaCostModifier = manaCostModifier;
            SpeedModifier    = speedModifier;
            CooldownModifier = cooldownModifier;
            LifetimeModifier = lifetimeModifier;
            Type             = type;
            Angle            = angle;
            Delay            = delay;
            Count            = count;
        }
        #endregion
    }
    #endregion
}