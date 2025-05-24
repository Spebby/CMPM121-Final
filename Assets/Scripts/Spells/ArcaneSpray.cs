using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using UnityEngine;
using Random = UnityEngine.Random;


namespace CMPM.Spells {
    public class ArcaneSpray : Spell {
        // protected readonly RPNString Count;
        protected readonly RPNString Spread;

        readonly SpellSplitModifier _splitModifier;

        public ArcaneSpray(SpellCaster owner, string name, RPNString manaCost, RPNString damage, Damage.Type damageType,
                           RPNString speed, RPNString cooldown, RPNString? lifetime, RPNString? count, uint icon,
                           RPNString countFormula, RPNString spreadFormula, int[] modifiers = null) : base(
            owner, name, manaCost, damage, damageType, speed, cooldown, lifetime, countFormula, icon, modifiers) {
            Count          = countFormula;
            Spread         = spreadFormula;
            // _splitModifier = new SpellSplitModifier(Count, Spread);
        }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, w,
                                                                        t - w, GetSpeed(), OnHit, GetLifetime());
            };

            // This is insanely dumb but you have to love it
            // _splitModifier.ModifyCast(this, ref castAction);
            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }
            // original: using a modifier to cast the spray
            // castAction(ProjectileType.STRAIGHT, where, target);

            // newer: using a loop to cast the spray
            Vector3 aiming_at = (target - where).normalized;
            float aim_angle = Mathf.Atan2(aiming_at.y, aiming_at.x) * Mathf.Rad2Deg;
            float _spread = Spread.Evaluate(GetRPNVariables());
            float spread_cone = 50f;
            for (int i = 0; i < GetCount(); i++)
            {
                float angle = aim_angle + Random.Range(0, spread_cone) * _spread - _spread / 2;

                Vector3 offsetTarget = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad),
                    0
                );

                castAction(ProjectileType.STRAIGHT, where, where+offsetTarget);
            }
            

            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }
    }
}