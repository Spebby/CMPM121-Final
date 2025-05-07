using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CMPM.Core;
using UnityEngine;
using Newtonsoft.Json.Linq;
using CMPM.DamageSystem;
using CMPM.Structures;
using CMPM.Utils;
using Newtonsoft.Json;


namespace CMPM.Spells {
    public static class SpellBuilder {
        static Hashtable<string, SpellData> _spellRegistry = new();
        
        public static void ParseSpellsJson(TextAsset spellsJson, TextAsset modifiersJson) {
            // Spells
            foreach (JToken _ in JToken.Parse(spellsJson.text)) {
                SpellData s = _.ToObject<SpellData>();
                _spellRegistry[s.Name] = s;
            }
            
            // Build Modifiers Library
            
        }
        

        //to randomly get a spell by picking a random key
        public static Spell MakeRandomSpell(string spellName, SpellCaster owner) {
            List<string> spellNames      = new(_spellRegistry.Keys);
            string       randomSpellName = spellNames[Random.Range(0, spellNames.Count)];
            return BuildSpell(randomSpellName, owner);
        }


        //to make the spell
        public static Spell BuildSpell(string spellName, SpellCaster owner) {
            SpellData              data         = _spellRegistry[spellName];
            Hashtable<string, int> RPNvars = new() {
                { "wave", GameManager.Instance.currentWave },
                { "power", owner.SpellPower},
            };
            
            uint[] modifiers = null;
            
            switch (data.Name) {
                case "Arcane Bolt":
                    return new ArcaneBolt(owner, data.Name, data.ManaCost, data.Damage.DamageRPN, data.DamageType, data.Cooldown,
                                     data.Icon, modifiers);
                case "Magic Missile":
                    return new MagicMissile(owner, data.Name, data.ManaCost, data.Damage.DamageRPN, data.DamageType, data.Cooldown,
                                            data.Icon, modifiers);
                case "Arcane Blast":
                    return new ArcaneBlast(owner, data.Name, data.ManaCost, data.Damage.DamageRPN, data.DamageType, data.Cooldown,
                                           data.Icon, modifiers);
                case "Arcane Spray":
                    return new ArcaneSpray(owner, data.Name, data.ManaCost, data.Damage.DamageRPN, data.DamageType, data.Cooldown,
                                           data.Icon, modifiers);
                //then it is a modifier spell, by recursively calling the BuildSpell method
                default: {
                    throw new System.Exception("Unknown spell name: " + spellName);
                }
            }
        }
    }
    
    #region JSON Parsing
    // TODO: At some point making these *all* RPN Strings may be beneficial simply for added flexibility would be cool.
    // Though that would be bad for performance lol
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct SpellData {
        #region Metadata
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("damage")]
        public string Description;
        [JsonProperty("icon")]
        public uint Icon;
        #endregion
       
        #region Stats
        public SpellDamageData Damage;
        [JsonConverter(typeof(RPNStringParser)), JsonProperty("damage")]
        public Damage.Type DamageType;
        
        [JsonConverter(typeof(RPNStringParser)), JsonProperty("mana_cost")]
        public RPNString ManaCost;
        [JsonConverter(typeof(RPNStringParser)), JsonProperty("cooldown"), Tooltip("In ms.")]
        public RPNString Cooldown;
       
        [JsonProperty("damage")]
        public ProjectileData Projectile;
        [JsonProperty("secondary_projectile")]
        public ProjectileData SecondaryProjectile;
        
        [JsonConverter(typeof(RPNStringParser)), JsonProperty("N")]
        public RPNString Count;
        [JsonProperty("spray")]
        public float Spray;
        #endregion
    }

    [JsonConverter(typeof(SpellDamageParser))]
    public struct SpellDamageData {
        public readonly RPNString DamageRPN;
        public readonly Damage.Type Type;
        
        public SpellDamageData(RPNString damageRPN, Damage.Type type) {
            DamageRPN = damageRPN;
            Type = type;
        }
    }
    
    [JsonConverter(typeof(ProjectileDataParser))]
    public readonly struct ProjectileData {
        public readonly string Trajectory;
        public readonly float Speed;
        public readonly uint Lifetime;
        public readonly int Sprite;

        public ProjectileData(string trajectory, float speed, int sprite, uint lifetime) {
            Trajectory = trajectory;
            Speed = speed;
            Lifetime = lifetime;
            Sprite = sprite;
        }
    }
    #endregion
}