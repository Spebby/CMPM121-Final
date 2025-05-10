using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class MysticRiver : ArcaneSpray {
        public MysticRiver(SpellCaster owner, string name, RPNString manaCost, RPNString damage, Damage.Type damageType,
                           RPNString speed, RPNString cooldown, RPNString? lifetime, uint icon,
                           RPNString countFormula, RPNString spreadFormula, int[] modifiers = null) : base(
            owner, name, manaCost, damage, damageType, speed, cooldown, lifetime, icon, countFormula, spreadFormula, modifiers) { }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                int   count   = (int)Count.Evaluate(GetRPNVariables());
                float delay   = GetCooldown() * 0.75f / count;
                CoroutineManager.Instance.Run(CastHelper(type, w, t, count, delay));
            };
            
            // This is insanely dumb but you have to love it
            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }

            castAction(ProjectileType.SINE, where, target);
            
            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }

        IEnumerator CastHelper(ProjectileType type, Vector3 where, Vector3 target, int count, float delay) {
            for (int i = 0; i < count; i++) {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, where, target - where, GetSpeed(), OnHit, GetLifetime());
                yield return new WaitForSeconds(delay);
            }
        }
    }
}