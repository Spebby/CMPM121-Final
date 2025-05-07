using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Structures;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    public abstract class Spell : IRPNEvaluator {
        public float LastCast { get; protected set; }
        public readonly SpellCaster Owner;
        protected Hittable.Team Team;
        
        #region Values
        public readonly string Name;
        protected RPNString ManaCost;
        protected RPNString DamageFormula;
        public readonly Damage.Type DamageType;
        protected RPNString Cooldown; // In Miliseconds
        readonly uint _iconIndex;
        #endregion

        protected readonly uint[] Modifiers;

        /// <summary>
        /// Spell Constructor
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="manaCost"></param>
        /// <param name="damage"></param>
        /// <param name="damageDamageType"></param>
        /// <param name="cooldown">Cooldown duration in miliseconds</param>
        /// <param name="icon">Index of the icon.</param>
        /// <param name="modifiers">List of modifier hashes. Null by default.</param>
        public Spell(SpellCaster owner, string name, RPNString manaCost, RPNString damage, Damage.Type damageDamageType, RPNString cooldown,
                     uint icon, uint[] modifiers = null) {
            Owner         = owner;
            Name          = name;
            ManaCost      = manaCost;
            DamageFormula = damage;
            DamageType          = damageDamageType;
            Cooldown      = cooldown;
            _iconIndex     = icon;

            Team     = owner.Team;
            LastCast = 0;

            Modifiers = modifiers;
        }

        public string GetName() {
            return Name;
        }

        public virtual int GetManaCost() {
            int baseManaCost = ManaCost.Evaluate(new Dictionary<string, int>{
                { "wave", GameManager.Instance.currentWave},
                { "power", Owner.SpellPower}
            });

            foreach (uint hash in Modifiers ?? Array.Empty<uint>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                baseManaCost = mod?.ModifyManaCost(this, baseManaCost) ?? 0;
            }
            
            return baseManaCost;
        }

        public virtual int GetDamage() {
            int baseDamage = DamageFormula.Evaluate(new Dictionary<string, int> {
                { "wave", GameManager.Instance.currentWave},
                { "power", Owner.SpellPower }
            });

            foreach (uint hash in Modifiers ?? Array.Empty<uint>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                baseDamage = mod?.ModifyDamage(this, baseDamage) ?? baseDamage;
            }
            
            return baseDamage;
        }

        public virtual float GetCooldown() {
            float baseCooldown = Cooldown.Evaluate(GetRPNVariables());
            foreach (uint hash in Modifiers ?? Array.Empty<uint>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                baseCooldown = mod?.ModifyCooldown(this, baseCooldown) ?? baseCooldown;
            }

            return baseCooldown;
        }

        public uint GetIcon() {
            return _iconIndex;
        }

        public bool IsReady() {
            return LastCast + GetCooldown() < Time.time;
        }

        public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<Vector3, Vector3> castAction = (w, t) => {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, ProjectileType.STRAIGHT, where,
                                                                        target - where, 15f, OnHit);
            };

            foreach (uint hash in Modifiers ?? Array.Empty<uint>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }
            
            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }

        protected virtual void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            Action<Hittable, Vector3> hitAction = (o, i) => {
                other.Damage(new Damage(GetDamage(), DamageType));
            };

            foreach (uint hash in Modifiers ?? Array.Empty<uint>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }
            
            other.Damage(new Damage(GetDamage(), DamageType));
        }

        public Hashtable<string, float> GetRPNVariables() {
            return new Hashtable<string, float> {
                { "wave", GameManager.Instance.currentWave },
                { "power", Owner.SpellPower}
            };
        }
    }
}