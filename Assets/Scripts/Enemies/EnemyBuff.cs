using System.Collections;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.Enemies {
    public class EnemyBuff : EnemyAction {
        readonly int _amount;
        readonly int _duration;

        public EnemyBuff(float cooldown, float range, int amount, int duration) : base(cooldown, range) {
            _amount   = amount;
            _duration = duration;
        }

        public EnemyBuff(float cooldown, float range, int amount) : base(cooldown, range) {
            _amount   = amount;
            _duration = -1;
        }

        protected override bool Perform(Transform target) {
            EnemyController healee = target.GetComponent<EnemyController>();

            healee.AddEffect("strength", _amount);
            if (_duration > 0) healee.StartCoroutine(Expire(healee));

            return true;
        }

        public IEnumerator Expire(EnemyController healee) {
            yield return new WaitForSeconds(_duration);
            healee.AddEffect("strength", -_amount);
        }
    }
}