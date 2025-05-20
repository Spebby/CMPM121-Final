using System;
using CMPM.DamageSystem;
using CMPM.Movement;
using CMPM.Spells;
using UnityEngine;


namespace CMPM.Core {
    public class EventBus {
        static EventBus _theInstance;

        public static EventBus Instance {
            get { return _theInstance ??= new EventBus(); }
        }

        #region Actions
        public event Action<Vector3, Damage, Hittable> OnDamage;
        public event Action<Vector3, int, Hittable> OnHeal;
        public event Action<EnemyController> OnEnemyDeath;
        public event Action OnPlayerMove;
        #endregion

        #region Callers
        public void DoDamage(Vector3 where, Damage dmg, Hittable target) {
            OnDamage?.Invoke(where, dmg, target);
        }

        public void DoHeal(Vector3 where, int amount, Hittable target) {
            OnHeal?.Invoke(where, amount, target);
        }

        public void DoEnemyDeath(EnemyController which) {
            OnEnemyDeath?.Invoke(which);
        }

        public void DoPlayerMove() {
            OnPlayerMove?.Invoke();
        }
    
        #endregion
    }
}
