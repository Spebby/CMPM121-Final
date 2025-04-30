using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using CMPM.Core;
using CMPM.DamageSystem;

namespace CMPM.Spells {
    public static class SpellBuilder {
        private static Dictionary<string, SpellData> spellsType;

        //to load the json file
        public static void LoadSpellsJson(TextAsset spellsText) {
            spellsType = new Dictionary<string, SpellData>();
            JObject json = JObject.Parse(spellsText.text);

            foreach (var property in json.Properties()) {
                SpellData spell = property.Value.ToObject<SpellData>();
                spellsType[property.Name] = spell;
            }
        }

        //to randomly get a spell by picking a random key
        public static Spell MakeRandomSpell(string spellName, SpellCaster owner) {
            List<string> spellNames = new List<string>(spellsType.Keys);
            string randomSpellName = spellNames[Random.Range(0, spellNames.Count)];
            return BuildSpell(randomSpellName, owner);
        }


        //to make the spell
        public static Spell BuildSpell(string spellName, SpellCaster owner) {
            SpellData data = spellsType[spellName];
            if (data.name == "Arcane Bolt") {
                return new Spell(owner) {
                    Name = data.name,
                    Damage = new Damage(
                        RPN.Evaluate(data.damage.amount, new Hashtable<string, float> { { "damage_multiplier", data.damage_multiplier } })
                    ),
                    ManaCost = RPN.Evaluate(data.mana_cost, new Hashtable<string, int> { {"mana_multiplier", data.mana_multiplier} }),
                    Cooldown = RPN.Evaluate(data.cooldown, new Hashtable<string, float> { { "cooldown_multiplier", data.cooldown_multiplier} }),
                    Projectile = new ProjectileData {
                        Trajectory = data.projectile.trajectory,
                        Speed = RPN.Evaluate(data.projectile.speed, new Hashtable<string, float> { { "speed_multiplier", data.speed_multiplier}}),
                        Sprite = data.projectile.sprite
                    }
                };
            }
            if (data.name == "Magic Missile") {
                return new Spell(owner) {
                    Name = data.name,
                    Damage = new Damage(
                        RPN.Evaluate(data.damage.amount, new Hashtable<string, float> { { "damage_multiplier", data.damage_multiplier } })
                    ),
                    ManaCost = RPN.Evaluate(data.mana_cost, new Hashtable<string, int> { {"mana_multiplier", data.mana_multiplier} }),
                    Cooldown = RPN.Evaluate(data.cooldown, new Hashtable<string, float> { {"cooldown_multiplier", data.cooldown_multiplier} }),
                    Projectile = new ProjectileData {
                        Trajectory = data.projectile.trajectory,
                        Speed = RPN.Evaluate(data.projectile.speed, new Hashtable<string, float> { {"speed_multiplier", data.speed_multiplier} }),
                        Sprite = data.projectile.sprite
                    }
                };
            }
            if (data.name == "Arcane Blast") {
                return new Spell(owner) {
                    Name = data.name,
                    Damage = new Damage(
                        RPN.Evaluate(data.damage.amount, new Hashtable<string, float> { { "damage_multiplier", data.damage_multiplier } })
                    ),
                    ManaCost = RPN.Evaluate(data.mana_cost, new Hashtable<string, int> { {"mana_multiplier", data.mana_multiplier} }),
                    Cooldown = RPN.Evaluate(data.cooldown, new Hashtable<string, float> { {"cooldown_multiplier", data.cooldown_multiplier} }),
                    Projectile = new ProjectileData {
                        Trajectory = data.projectile.trajectory,
                        Speed = RPN.Evaluate(data.projectile.speed, new Hashtable<string, float> { {"speed_multiplier", data.speed_multiplier} }),
                        Sprite = data.projectile.sprite
                    },
                    SecondaryProjectile = new ProjectileData {
                        Trajectory = data.secondary_projectile.trajectory,
                        Speed = RPN.Evaluate(data.secondary_projectile.speed, new Hashtable<string, float> { {"speed_multiplier", data.speed_multiplier} }),
                        Lifetime = RPN.Evaluate(data.secondary_projectile.lifetime, new Hashtable<string, float> { {"lifetime_multiplier", data.lifetime} }),
                        Sprite = data.secondary_projectile.sprite
                    }
                };
            }
            if (data.name == "Arcane Spray") {
                return new Spell(owner) {
                    Name = data.name,
                    Damage = new Damage(
                        RPN.Evaluate(data.damage.amount, new Hashtable<string, float> { { "damage_multiplier", data.damage_multiplier } })
                    ),
                    ManaCost = RPN.Evaluate(data.mana_cost, new Hashtable<string, int> { {"mana_multiplier", data.mana_multiplier} }),
                    Cooldown = RPN.Evaluate(data.cooldown, new Hashtable<string, float> { {"cooldown_multiplier", data.cooldown_multiplier} }),
                    Projectile = new ProjectileData {
                        Trajectory = data.projectile.trajectory,
                        Speed = RPN.Evaluate(data.projectile.speed, new Hashtable<string, float> { {"speed_multiplier", data.speed_multiplier} }),
                        Sprite = data.projectile.sprite
                    },
                    SecondaryProjectile = new ProjectileData {
                        Trajectory = data.secondary_projectile.trajectory,
                        Speed = RPN.Evaluate(data.secondary_projectile.speed, new Hashtable<string, float> { {"speed_multiplier", data.speed_multiplier} }),
                        Lifetime = RPN.Evaluate(data.secondary_projectile.lifetime, new Hashtable<string, float> { {"lifetime_multiplier", data.lifetime} }),
                        Sprite = data.secondary_projectile.sprite
                    }
                };
            }
            //then it is a modifier spell, by recursively calling the BuildSpell method
            else if (data.name == "damage_amp" || spellName == "speed_amp" || spellName == "doubler" || spellName == "splitter" || spellName == "chaos" || spellName == "homing"){
                //Spell baseSpell = BuildSpell("Arcane Bolt", owner); 
                return new ModifierSpell(baseSpell, data);
            }
        }
    }

    //used https://refactoring.guru/design-patterns/decorator/csharp/example to know how to create a decorator
    //used Markus Eger's Lecture 6 psudocode for spell decorator to create a ModiferSpell decorator
    public class ModifierSpell : Spell {
        private readonly Spell innerSpell;
        private readonly Spell modifiers;

        public ModifierSpell(Spell innerSpell, Spell modifiers) : base(innerSpell.Owner) {
            this.innerSpell = innerSpell;
            this.modifiers = modifiers;
        }
        public override string GetName() {
            return innerSpell.GetName();
        }

        public override int GetManaCost() {
            return (int)(innerSpell.GetManaCost() * modifiers.mana_multiplier + modifiers.mana_adder);
        }

        public override int GetDamage() {
            return (int)(innerSpell.GetDamage() * modifiers.damage_multiplier);
        }

        public override float GetCooldown() {
            return innerSpell.GetCooldown() * modifiers.cooldown_multiplier;
        }

        public override int GetChildCount(){
            return 1;
        }
        public void AddChild(){

        }
    }
    
    public class SpellData {
        public string name;
        public string description;
        public int icon;

        public DamageData[] damage;
        public float mana_cost;
        public float cooldown;
        public ProjectileData[] projectile;
        public ProjectileData[] secondary_projectile;

        public float secondary_damage;
        public float spray;
        public float N;
        public float damage_multiplier = 1f;
        public float mana_multiplier = 1f;
        public float cooldown_multiplier = 1f;
        public float speed_multiplier = 1f;
        public float mana_adder = 0f;
        public float delay = 0f;
        public string projectile_trajectory;
        public float angle = 0f;
    }

    public class DamageData {
        public string amount;
        public string type;
    }

    public class ProjectileData {
        public string trajectory;
        public string speed;
        public string lifetime;
        public int sprite;
    }
}