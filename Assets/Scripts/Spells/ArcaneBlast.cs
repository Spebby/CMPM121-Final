using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class ArcaneBlast : Spell {
        protected readonly ProjectileData Secondary;
        protected readonly RPNString Count;

        public ArcaneBlast(SpellCaster owner, string name, RPNString manaCost, RPNString damage,
                           Damage.Type damageDamageType, RPNString speed, RPNString cooldown, RPNString? lifetime,
                           RPNString count, ProjectileData secondary,
                           uint icon, int[] modifiers = null) : base(owner, name, manaCost, damage, damageDamageType,
                                                                     speed, cooldown, lifetime, icon, modifiers) {
            // do shit with secondary
            Count     = count;
            Secondary = secondary;
        }

        public virtual int GetSecondaryCount() {
            return (int)Count.Evaluate(GetRPNVariables());
        }
        //public virtual int GetSecondaryCount() => ApplyModifiers((int)Count.Evaluate(GetRPNVariables()),  (mod, val) => mod.Modify)

        protected virtual IEnumerator CastSecondary(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                float speed = ApplyModifiers(Speed.Evaluate(GetRPNVariables()),
                                             (mod, val) => mod.ModifySpeed(this, val));
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, w,
                                                                        t - w, speed, base.OnHit, GetLifetime());
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }

            castAction(Secondary.Trajectory, where, target);

            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }

        protected override void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            Action<Hittable, Vector3, Damage.Type> hitAction = (o, i, t) => {
                o.Damage(new Damage(GetDamage(), t));

                int   count     = GetSecondaryCount();
                float angleStep = 360f / count;
                for (int index = 0; index < count; index++) {
                    float angle = angleStep * index;
                    // direction in the XY-plane
                    Vector3 dir = new(
                        Mathf.Cos(Mathf.Deg2Rad * angle),
                        Mathf.Sin(Mathf.Deg2Rad * angle),
                        0f
                    );
                    CoroutineManager.Instance.Run(CastSecondary(i, dir, Owner.Team));
                }
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }

            hitAction(other, impact, DamageType);
        }
    }
}