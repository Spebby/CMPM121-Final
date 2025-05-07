using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class ArcaneBlast : Spell {
        public ArcaneBlast(SpellCaster owner, string name, RPNString manaCost, RPNString damage, Damage.Type damageDamageType, RPNString cooldown,
                          uint icon, uint[] modifiers = null) : base(owner, name, manaCost, damage, damageDamageType, cooldown, icon, modifiers) {
            
        }

        
        protected override void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            Action<Hittable, Vector3> hitAction = (o, i) => {
                other.Damage(new Damage(GetDamage(), DamageType));
                asdasd
            };

            foreach (uint hash in Modifiers ?? Array.Empty<uint>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }
            
            other.Damage(new Damage(GetDamage(), DamageType));
        } 
    }
}