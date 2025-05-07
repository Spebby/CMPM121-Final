using System;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.Spells {
    public interface ISpellModifier {
        void ModifyCast(Spell spell, ref Action<Vector3, Vector3> original);
        void ModifyHit(Spell spell, ref Action<Hittable, Vector3> original);
        
        int ModifyDamage(Spell spell, int baseDamage);
        int ModifyManaCost(Spell spell, int baseMana);
        float ModifySpeed(Spell spell, float baseSpeed);
        float ModifyCooldown(Spell spell, float cooldown);
        float  ModifyLifetime(Spell spell, float lifetime);
    }
}