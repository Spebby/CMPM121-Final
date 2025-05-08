using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class ArcaneSpray : Spell {
        protected readonly RPNString Count;
        protected readonly RPNString Spread;

        readonly SpellSplitModifier _splitModifier;

        public ArcaneSpray(SpellCaster owner, string name, RPNString manaCost, RPNString damage, Damage.Type damageType,
                           RPNString speed, RPNString cooldown, RPNString? lifetime, uint icon,
                           RPNString countFormula, RPNString spreadFormula, int[] modifiers = null) : base(
            owner, name, manaCost, damage, damageType, speed, cooldown, lifetime, icon, modifiers) {
            Count          = countFormula;
            Spread         = spreadFormula;
            _splitModifier = new SpellSplitModifier(Count, Spread);
        }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            Action<ProjectileType, Vector3, Vector3> castAction = (type, w, t) => {
                GameManager.Instance.ProjectileManager.CreateProjectile(0, type, w,
                                                                        t - w, GetSpeed(), OnHit);
            };
            
            // This is insanely dumb but you have to love it
            _splitModifier.ModifyCast(this, ref castAction);
            foreach (int hash in Modifiers ?? Array.Empty<int>()) {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyCast(this, ref castAction);
            }

            castAction(ProjectileType.STRAIGHT, where, target);
            
            LastCast = Time.time;
            yield return new WaitForEndOfFrame();
        }
    }
}