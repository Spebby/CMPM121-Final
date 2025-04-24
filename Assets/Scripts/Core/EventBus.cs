using System;
using CMPM.DamageSystem;
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
}