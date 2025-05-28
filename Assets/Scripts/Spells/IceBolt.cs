using System;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Status;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class IceBolt : Spell {
        protected readonly RPNString SlowFactor;
        protected readonly RPNString TimeSlowed;
        readonly SpellStatusModifier _slowModifier;
        
        
        public IceBolt(SpellCaster owner, string name, RPNString manaCost, RPNString damage,
                       Damage.Type damageDamageType, RPNString speed, RPNString hitcap,
                       RPNString cooldown, RPNString? lifetime, RPNString? count,
                       RPNString slowFactor, RPNString timeSlowed,
                       uint icon, int[] modifiers = null) : base(owner, name, manaCost, damage, damageDamageType,
                                                                 speed, hitcap, cooldown, lifetime, count, icon,
                                                                 modifiers) {
            TimeSlowed   = timeSlowed;
            SlowFactor   = slowFactor;
            _slowModifier = new SpellStatusModifier((entity) => new SlowStatus(entity, TimeSlowed, SlowFactor));
        }

        protected virtual float GetSlowFactor() {
            return SlowFactor.Evaluate(GetRPNVariables());
        }

        protected virtual float GetTimeSlowed() {
            return TimeSlowed.Evaluate(GetRPNVariables());
        }

        protected override void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            Action<Hittable, Vector3, Damage.Type> hitAction = (o, _, type) => {
                o.Damage(new Damage(GetDamage(), type));
            };

            // I know you don't care for this kevin: the rationale here is that since this *is an status effect* it really
            // should be using the status effect system, which is integrated w/ spell modifiers already for Relics, etc.
            _slowModifier.ModifyHit(this, ref hitAction);
            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }

            hitAction(other, impact, DamageType);;
        }
    }
}