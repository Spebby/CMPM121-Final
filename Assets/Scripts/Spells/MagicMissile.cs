using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class MagicMissile : Spell {
        public MagicMissile(SpellCaster owner, string name, RPNString manaCost, RPNString damage,
                            Damage.Type damageDamageType, RPNString speed, RPNString hitcap,
                            RPNString cooldown, RPNString? lifetime,
                            RPNString? count,
                            uint icon, int[] modifiers = null) : base(owner, name, manaCost, damage, damageDamageType,
                                                                      speed, hitcap, cooldown, lifetime, count, icon, modifiers) { }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, w,
                                                                        t - w, GetSpeed(), OnHit, GetHitCap());
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }

            castAction(ProjectileType.HOMING, where, target);

            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }
    }
}