using CMPM.Movement;
using UnityEngine;


namespace CMPM.Enemies {
    public class EnemyAction {
        #region Publics
        public float LastUse;
        public readonly float Cooldown;
        public readonly float Range;

        public EnemyController Enemy;
        #endregion

        public EnemyAction(float cooldown, float range) {
            Cooldown = cooldown;
            Range    = range;
        }

        public bool Ready() {
            return LastUse + Cooldown < Time.time;
        }

        public bool CanDo(Transform target) {
            return Ready() && InRange(target);
        }

        public bool InRange(Transform target) {
            // add some tolerance
            return (target.position - Enemy.transform.position).magnitude <= Range * 1.1f;
        }

        public bool Do(Transform target) {
            if (!CanDo(target)) return false;
            LastUse = Time.time;
            return Perform(target);
        }

        protected virtual bool Perform(Transform target) {
            return false;
        }
    }
}