using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class ArcaneBolt : Spell {
        public ArcaneBolt(SpellCaster owner, string name, RPNString manaCost, RPNString damage, Damage.Type damageDamageType, RPNString cooldown,
                          uint icon, uint[] modifiers = null) : base(owner, name, manaCost, damage, damageDamageType, cooldown, icon, modifiers) {
            
        }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<Vector3, Vector3> castAction = (w, t) => {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, ProjectileType.STRAIGHT, where,
                                                                        target - where, 15f, OnHit);
            };

            foreach (uint hash in Modifiers ?? Array.Empty<uint>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }

            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }
    }
}