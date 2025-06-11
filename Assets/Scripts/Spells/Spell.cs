using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Spells {
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    public abstract class Spell : IRPNEvaluator {
        public float LastCast { get; protected set; }
        protected readonly SpellCaster Owner;
        protected Hittable.Team Team;

        #region Values
        public readonly string Name;
        protected RPNString ManaCost;
        protected RPNString DamageFormula;
        public readonly Damage.Type DamageType;
        protected RPNString Speed;
        protected RPNString Cooldown; // In Milliseconds
        protected RPNString HitCap;
        protected RPNString? Lifetime;
        protected RPNString? Count;
        readonly uint _iconIndex;
        #endregion

        protected int[] Modifiers;

        /// <summary>
        /// Spell Constructor
        /// </summary>
        /// <param name="owner">Spell Caster that owns this spell object.</param>
        /// <param name="name">Name of the spell.</param>
        /// <param name="manaCost">RPN Formula for calculating Mana Cost.</param>
        /// <param name="damage">RPN Formula for calculating damage.</param>
        /// <param name="damageDamageType">Damage Type used for the projectile.</param>
        /// <param name="speed">RPN Formula for calculating projectile speed.</param>
        /// <param name="hitcap">Number of enemies projectiles can pierce.</param>
        /// <param name="cooldown">RPN Formula for calculating usage cooldown.</param>
        /// <param name="lifetime">RPN Formula for calculating projectile lifetime.</param>
        /// <param name="count"></param>
        /// <param name="icon">Index of the icon.</param>
        /// <param name="modifiers">List of modifier hashes. Null by default.</param>
        public Spell(SpellCaster owner,
                     string name,
                     RPNString manaCost,
                     RPNString damage,
                     Damage.Type damageDamageType,
                     RPNString speed,
                     RPNString hitcap,
                     RPNString cooldown,
                     RPNString? lifetime,
                     RPNString? count,
                     uint icon,
                     int[] modifiers = null) {
            Owner         = owner;
            Name          = name;
            ManaCost      = manaCost;
            DamageFormula = damage;
            DamageType    = damageDamageType;
            Speed         = speed;
            HitCap        = hitcap;
            Cooldown      = cooldown;
            Lifetime      = lifetime;
            Count         = count;
            _iconIndex    = icon;

            Team     = owner.Team;
            LastCast = 0;

            Modifiers = modifiers ?? Array.Empty<int>();
        }

        public string GetName() {
            string baseStr = Name;
            foreach (int modifier in Modifiers ?? Array.Empty<int>()) {
                SpellModifierData data = SpellModifierDataRegistry.Get(modifier);
                if (data.Name == null) continue;
                char[] adjName = data.Name.ToCharArray();
                adjName[0]     = char.ToUpper(adjName[0]);
                baseStr        = $"{new string(adjName)} {baseStr}";
            }

            return baseStr;
        }

        public string GetDescription() {
            string baseStr = $"base: {SpellRegistry.Get(Name.GetHashCode()).Description}\n";
            foreach (int modifier in Modifiers ?? Array.Empty<int>()) {
                if (!SpellModifierDataRegistry.TryGet(modifier, out SpellModifierData data)) continue;
                baseStr += $"{data.Name}: {data.Description}\n";
            }

            return baseStr;
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

        public virtual int GetManaCost() {
            return ApplyModifiers((int)ManaCost.Evaluate(GetRPNVariables()),
                                  (mod, val) => mod.ModifyManaCost(this, val));
        }

        public virtual int GetDamage() {
            return ApplyModifiers((int)DamageFormula.Evaluate(GetRPNVariables()),
                                  (mod, val) => mod.ModifyDamage(this, val));
        }

        public virtual float GetSpeed() {
            return ApplyModifiers(Speed.Evaluate(GetRPNVariablesSafe()),
                                  (mod, val) => mod.ModifySpeed(this, val));
        }
        
        public virtual int GetHitCap() {
            return (int)ApplyModifiers(HitCap.Evaluate(GetRPNVariablesSafe()),
                                  (mod, val) => mod.ModifyHitCap(this, (int)val));
        }

        public virtual float GetCooldown() {
            return ApplyModifiers(Cooldown.Evaluate(GetRPNVariables()),
                                  (mod, val) => mod.ModifyCooldown(this, val));
        }

        public virtual float GetLifetime() {
            return ApplyModifiers(Lifetime?.Evaluate(GetRPNVariables()) ?? 9999f,
                                  (mod, val) => mod.ModifyLifetime(this, val));
        }

        public virtual int GetCount(){
            return Mathf.RoundToInt(ApplyModifiers(Count?.Evaluate(GetRPNVariables()) ?? 0,
                                    (mod, val) => mod.ModifyCount(this, (float)val)));
        }
        #endregion

        public uint GetIcon() {
            return _iconIndex;
        }

        public bool IsReady() {
            return LastCast + GetCooldown() < Time.time;
        }

        public void AddModifiers(int[] additions) {
            int[] newModifiers = new int[Modifiers.Length + additions.Length];
            Array.Copy(Modifiers, 0, newModifiers, 0, Modifiers.Length);
            Array.Copy(additions, 0, newModifiers, Modifiers.Length, additions.Length);
            Modifiers = newModifiers;
        }

        public void RemoveModifiers(int[] removals) {
            int[]  temp    = new int[Modifiers.Length];
            bool[] removed = new bool[removals.Length];

            int write = 0;

            foreach (int t in Modifiers) {
                bool matched = false;
                for (int j = 0; j < removals.Length; j++) {
                    if (removed[j] || t != removals[j]) continue;
                    removed[j] = true;
                    matched    = true;
                    break;
                }

                if (matched) continue;
                temp[write++] = t;
            }

            // If nothing was removed, return.
            if (write == Modifiers.Length) return;
            int[] newModifiers = new int[write];
            Array.Copy(temp, newModifiers, write);
            Modifiers = newModifiers;
        }

        public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, w,
                                                                        t - w, GetSpeed(), OnHit, GetHitCap());
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }

            castAction(ProjectileType.STRAIGHT, where, target);

            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }

        protected virtual void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            Action<Hittable, Vector3, Damage.Type> hitAction = (o, _, type) => {
                o.Damage(new Damage(GetDamage(), type));
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }

            hitAction(other, impact, DamageType);
        }

        // NOTE: this **CANNOT** be used on speed, as you'll end up with a circular calculation.
        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentFloor },
                { "power", Owner.SpellPower },
                { "speed", GetSpeed() }
            };
        }

        public SerializedDictionary<string, float> GetRPNVariablesSafe() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentFloor },
                { "power", Owner.SpellPower }
            };
        }
    }
}