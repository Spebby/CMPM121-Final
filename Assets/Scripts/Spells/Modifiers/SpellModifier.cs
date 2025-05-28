using System;
using CMPM.DamageSystem;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Spells.Modifiers {
    public abstract class SpellModifier : ISpellModifier {
        #region Protected Properties
        protected readonly RPNString? DamageModifier;
        protected readonly RPNString? ManaModifier;
        protected readonly RPNString? SpeedModifier;
        protected readonly RPNString? HitCapModifier;
        protected readonly RPNString? CooldownModifier;
        protected readonly RPNString? LifetimeModifier;
        protected readonly RPNString? CountModifier;
        #endregion

        public SpellModifier(RPNString? damageModifier = null,
                             RPNString? manaModifier = null,
                             RPNString? speedModifier = null,
                             RPNString? hitCapModifier = null,
                             RPNString? cooldownModifier = null,
                             RPNString? lifetimeModifier = null,
                             RPNString? countModifier = null) {
            DamageModifier   = damageModifier;
            ManaModifier     = manaModifier;
            SpeedModifier    = speedModifier;
            HitCapModifier   = hitCapModifier;
            CooldownModifier = cooldownModifier;
            LifetimeModifier = lifetimeModifier;
            CountModifier    = countModifier;
        }

        public virtual void ModifyCast(Spell spell, ref Action<ProjectileType, Vector3, Vector3> original) { }
        public virtual void ModifyHit(Spell spell, ref Action<Hittable, Vector3, Damage.Type> original) { }

        protected virtual float ApplyModifier(Spell spell, RPNString? modifier, float value,
                                              SerializedDictionary<string, float> table) {
            // The RPN Strings themselves have "value" worked into them in this case, this step is done during parsing.
            table["value"] = value;
            return modifier?.Evaluate(table) ?? value;
        }

        public virtual int ModifyDamage(Spell spell, int baseDamage) {
            return (int)ApplyModifier(spell, DamageModifier, baseDamage, spell.GetRPNVariables());
        }

        public virtual int ModifyManaCost(Spell spell, int baseMana) {
            return (int)ApplyModifier(spell, ManaModifier, baseMana, spell.GetRPNVariables());
        }

        public virtual float ModifySpeed(Spell spell, float baseSpeed) {
            return ApplyModifier(spell, SpeedModifier, baseSpeed, spell.GetRPNVariablesSafe());
        }
        
        public virtual int ModifyHitCap(Spell spell, int baseHitCap) {
            return (int)ApplyModifier(spell, HitCapModifier, baseHitCap, spell.GetRPNVariablesSafe());
        }

        public virtual float ModifyCooldown(Spell spell, float cooldown)
        {
            return ApplyModifier(spell, CooldownModifier, cooldown, spell.GetRPNVariables());
        }

        public virtual float ModifyLifetime(Spell spell, float lifetime) {
            return ApplyModifier(spell, LifetimeModifier, lifetime, spell.GetRPNVariables());
        }
        
        public virtual int ModifyCount(Spell spell, float count) {
            return (int)ApplyModifier(spell, CountModifier, count, spell.GetRPNVariables());
        }
    }
}
