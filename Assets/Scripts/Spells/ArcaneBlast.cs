using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class ArcaneBlast : Spell {
        public ArcaneBlast(SpellCaster owner,string name, RPNString manaCost, RPNString damage,
                           Damage.Type damageDamageType, RPNString speed, RPNString cooldown, RPNString? lifetime, ProjectileData secondary,
                           uint icon, int[] modifiers = null) : base(owner, name, manaCost, damage, damageDamageType,
                                                                      speed, cooldown, lifetime, icon, modifiers) {
            // do shit with secondary
        }

        
        protected override void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            Action<Hittable, Vector3> hitAction = (o, i) => {
                o.Damage(new Damage(GetDamage(), DamageType));
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }
            
            other.Damage(new Damage(GetDamage(), DamageType));
        } 
    }
}