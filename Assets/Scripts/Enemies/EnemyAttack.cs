using CMPM.Core;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.Enemies {
    public class EnemyAttack : EnemyAction {
        #region Publics
        public readonly float StrengthFactor;
        public readonly int AttackDamage;
        #endregion

        public EnemyAttack(float cooldown, float range, int damage, float strengthFactor) : base(cooldown, range) {
            AttackDamage   = damage;
            StrengthFactor = strengthFactor;
        }

        protected override bool Perform(Transform target) {
            int amount = AttackDamage;
            amount += Mathf.RoundToInt(Enemy.GetEffect("strength") * StrengthFactor);
            GameManager.Instance.PlayerController.HP
                       .Damage(new Damage(amount, Damage.Type.PHYSICAL));
            return true;
        }
    }
}