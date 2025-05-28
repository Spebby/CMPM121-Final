using CMPM.DamageSystem;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.Core {
    public abstract class Entity : MonoBehaviour {
        public Hittable HP;
        public Unit unit;
        public float Speed { get;  protected set; }

        public void ModifySpeed(float c) {
            Speed = Mathf.Max(Speed + c, 1);
        }
        public Hittable.Team Team => HP.team;
    }
}