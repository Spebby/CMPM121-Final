using System;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.Spells {
    public abstract class SpellModifier : ISpellModifier {
        #region Protected Properties
        protected readonly float DamageMultiplier;
        protected readonly float ManaMultiplier;
        protected readonly float SpeedMultiplier;
        protected readonly float CooldownMultiplier;
        protected readonly float LifetimeMultiplier;
        #endregion
        
        public SpellModifier(float damageMultiplier = 1f,
                             float manaMultiplier = 1f,
                             float speedMultiplier = 1f,
                             float cooldownMultiplier = 1f,
                             float lifetimeMultiplier = 1f) {
            DamageMultiplier   = damageMultiplier;
            ManaMultiplier     = manaMultiplier;
            SpeedMultiplier    = speedMultiplier;
            CooldownMultiplier = cooldownMultiplier;
            LifetimeMultiplier = lifetimeMultiplier;
        }
        
        public virtual void ModifyCast(Spell spell, ref Action<Vector3, Vector3> original) { }
        public virtual void ModifyHit(Spell spell, ref Action<Hittable, Vector3> original) { }
        
        public virtual int ModifyDamage(Spell spell, int baseDamage) => (int)(baseDamage * DamageMultiplier);
        public virtual int ModifyManaCost(Spell spell, int baseMana) => (int)(baseMana * ManaMultiplier);
        public virtual float ModifySpeed(Spell spell, float baseSpeed) => baseSpeed * SpeedMultiplier;
        public virtual float ModifyCooldown(Spell spell, float cooldown) => cooldown * CooldownMultiplier;
        public virtual float ModifyLifetime(Spell spell, float lifetime) => lifetime * LifetimeMultiplier;
    }
}