using CMPM.DamageSystem;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.Enemies {
    public class EnemyHeal : EnemyAction {
        readonly int _amount;

        public EnemyHeal(float cooldown, float range, int amount) : base(cooldown, range) {
            _amount = amount;
        }

        protected override bool Perform(Transform target) {
            EnemyController healee = target.GetComponent<EnemyController>();

            // some targets might have a debuff
            if (healee.GetEffect("noheal") > 0) return false;
            Enemy.HP.Damage(new Damage(1, Damage.Type.DARK));
            healee.HP.Heal(_amount);
            return true;
        }
    }
}