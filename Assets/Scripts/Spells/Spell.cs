using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
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
        protected RPNString  ManaCost;
        protected RPNString  DamageFormula;
        public readonly Damage.Type DamageType;
        protected RPNString  Speed;
        protected RPNString  Cooldown; // In Miliseconds
        protected RPNString? Lifetime;
        readonly uint _iconIndex;
        #endregion

        protected readonly int[] Modifiers;

        /// <summary>
        /// Spell Constructor
        /// </summary>
        /// <param name="owner">Spell Caster that owns this spell object.</param>
        /// <param name="name">Name of the spell.</param>
        /// <param name="manaCost">RPN Formula for calculating Mana Cost.</param>
        /// <param name="damage">RPN Formula for calculating damage.</param>
        /// <param name="damageDamageType">Damage Type used for the projectile.</param>
        /// <param name="speed">RPN Formula for calculating projectile speed.</param>
        /// <param name="cooldown">RPN Formula for calculating usage cooldown.</param>
        /// <param name="lifetime">RPN Formula for calculating projectile lifetime.</param>
        /// <param name="icon">Index of the icon.</param>
        /// <param name="modifiers">List of modifier hashes. Null by default.</param>
        public Spell(SpellCaster owner,
                     string name,
                     RPNString  manaCost,
                     RPNString  damage,
                     Damage.Type damageDamageType,
                     RPNString  speed,
                     RPNString  cooldown,
                     RPNString? lifetime,
                     uint icon,
                     int[] modifiers = null) {
            Owner         = owner;
            Name          = name;
            ManaCost      = manaCost;
            DamageFormula = damage;
            DamageType    = damageDamageType;
            Speed         = speed;
            Cooldown      = cooldown;
            Lifetime      = lifetime;
            _iconIndex    = icon;

            Team     = owner.Team;
            LastCast = 0;

            Modifiers = modifiers;
        }

        public string GetName() {
            return Name;
        }

        public string GetDescription() {
            return SpellRegistry.Get(Name.GetHashCode()).Description;
        }

        #region Stat Getters
        protected virtual T ApplyModifiers<T>(T baseValue, Func<ISpellModifier, T, T> apply) {
            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                if (mod != null)
                    baseValue = apply(mod, baseValue);
            }
            return baseValue;
        }

        public virtual int GetManaCost() => ApplyModifiers((int)ManaCost.Evaluate(GetRPNVariables()), (mod, val) => mod.ModifyManaCost(this, val));

        public virtual int GetDamage() {
            SerializedDictionary<string, float> dict = GetRPNVariables();
            dict["speed"] = GetSpeed(); // This has to be done here to prevent a recursive loop that crashes the game
            return ApplyModifiers((int)DamageFormula.Evaluate(dict), (mod, val) => mod.ModifyDamage(this, val));
        }

        public virtual float GetSpeed() => ApplyModifiers(Speed.Evaluate(GetRPNVariables()), (mod, val) => mod.ModifySpeed(this, val));
        public virtual float GetCooldown() => ApplyModifiers(Cooldown.Evaluate(GetRPNVariables()), (mod, val) => mod.ModifyCooldown(this, val));
        public virtual float GetLifetime() => ApplyModifiers(Lifetime?.Evaluate(GetRPNVariables()) ?? 9999f, (mod, val) => mod.ModifyLifetime(this, val));
        #endregion

        public uint GetIcon() {
            return _iconIndex;
        }

        public bool IsReady() {
            return LastCast + GetCooldown() < Time.time;
        }

        public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                // Which is always "0" by default
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, w,
                                                                        t - w, GetSpeed(), OnHit);
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }
            
            castAction(ProjectileType.STRAIGHT, target, where);
            
            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }

        protected virtual void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            Action<Hittable, Vector3, Damage.Type> hitAction = (o, i, type) => {
                o.Damage(new Damage(GetDamage(), type));
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }
            
            hitAction(other, impact, DamageType);
        }

        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave",  GameManager.Instance.currentWave },
                { "power", Owner.SpellPower }
            };
        }
    }
}
