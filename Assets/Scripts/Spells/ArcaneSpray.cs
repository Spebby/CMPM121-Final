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
        // readonly SpellSplitModifier _splitModifier;

        public ArcaneSpray(SpellCaster owner, string name, RPNString manaCost, RPNString damage,
                           Damage.Type damageType, RPNString speed, RPNString hitcap,
                           RPNString cooldown, RPNString? lifetime,
                           uint icon, RPNString countFormula, RPNString spreadFormula,
                           int[] modifiers = null) : base(owner, name, manaCost, damage,
                                                          damageType, speed, hitcap, cooldown,
                                                          lifetime, countFormula, icon, modifiers) {
            Count          = countFormula;
            Spread         = spreadFormula;
            // _splitModifier = new SpellSplitModifier(Count, Spread);
        }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, w,
                                                                        t - w, GetSpeed(), OnHit, GetHitCap(), GetLifetime());
            };
            
            // This is insanely dumb but you have to love it
            // _splitModifier.ModifyCast(this, ref castAction);
            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }
            // original: using a modifier to cast the spray
            // castAction(ProjectileType.STRAIGHT, where, target);

            // Thom: I'm not sure if I like this.
            // Spread is static which could be fine, we'll have to see.
            // my main reason for reusing the modifier here was to have "consistency" in the game logic between
            // how the base spell and the modifier worked, and it seemed redundant to hardcode the functionality
            // into the spell. I'll have ot see how it handles in-game though.
            
            // newer: using a loop to cast the spray
            Vector3     aimingAt = (target - where).normalized;
            float       aimAngle = Mathf.Atan2(aimingAt.y, aimingAt.x) * Mathf.Rad2Deg;
            float       spread   = Spread.Evaluate(GetRPNVariables());
            const float CONE     = 50f;
            for (int i = 0; i < GetCount(); i++) {
                float angle = aimAngle + Random.Range(0, CONE) * spread - spread / 2;

                Vector3 offsetTarget = new(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad),
                    0
                );

                castAction(ProjectileType.STRAIGHT, where, where + offsetTarget);
            }
            

            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }
    }
}