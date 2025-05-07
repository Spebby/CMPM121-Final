using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class ArcaneSpray : Spell {
        protected int Count;
        protected float Spray;
        
        public ArcaneSpray(SpellCaster owner, string name, RPNString manaCost, RPNString damage, Damage.Type damageType, RPNString cooldown, uint icon,
                          uint[] modifiers = null) : base(owner, name, manaCost, damage, damageType, cooldown, icon, modifiers) {
            
        }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            GameManager.Instance.ProjectileManager.CreateProjectile(0, ProjectileType.STRAIGHT, where, target - where, 15f, OnHit);
            yield return new WaitForEndOfFrame();
        }
    }
}