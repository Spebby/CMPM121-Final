using System;
using CMPM.DamageSystem;
using CMPM.Movement;
using UnityEngine;


public class EventBus {
    static EventBus _theInstance;

    public static EventBus Instance {
        get { return _theInstance ??= new EventBus(); }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;

    public void DoDamage(Vector3 where, Damage dmg, Hittable target) {
        OnDamage?.Invoke(where, dmg, target);
    }

    public event Action<Vector3, int, Hittable> OnHeal;

    public void DoHeal(Vector3 where, int amount, Hittable target) {
        OnHeal?.Invoke(where, amount, target);
    }

    public event Action<EnemyController> OnEnemyDeath;

    public void DoEnemyDeath(EnemyController which) {
        OnEnemyDeath?.Invoke(which);
    }
}