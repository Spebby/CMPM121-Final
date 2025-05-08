using System;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells.Modifiers {
    public class SpellProjectileModifier : SpellModifier {
        protected readonly ProjectileType Type;

        public SpellProjectileModifier(ProjectileType type,
                                       RPNString? damageModifier   = null,
                                       RPNString? manaModifier     = null,
                                       RPNString? speedModifier    = null,
                                       RPNString? cooldownModifier = null,
                                       RPNString? lifetimeModifier = null,
                                       Operator op                 = Operator.MULTIPLIER) 
            : base(damageModifier, manaModifier, 
                   speedModifier, cooldownModifier,
                   lifetimeModifier, op) {
            Type = type;
        }

        public override void ModifyCast(Spell spell, ref Action<ProjectileType, Vector3, Vector3> original) {
            Action<ProjectileType, Vector3, Vector3> prev = original;
            original = (type, where, target) => {
                prev (Type, where, target);
            };
        }
    }
}