using System;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Status;
using UnityEngine;


namespace CMPM.Spells.Modifiers {
    public class SpellStatusModifier : SpellModifier {
        protected readonly Func<Entity, IStatusEffect> EffectFactory;

        public SpellStatusModifier(Func<Entity, IStatusEffect> factory) {
            EffectFactory = factory;
        } 
        
        public override void ModifyHit(Spell spell, ref Action<Hittable, Vector3, Damage.Type> original) {
            Action<Hittable, Vector3, Damage.Type> prev = original;
            original = (hittable, pos, damage) => {
                IStatusEffect inst = EffectFactory(hittable.Owner);
                inst.ApplyStatus();
                prev(hittable, pos, damage);
            };
        }
    }
}