using UnityEngine;


namespace CMPM.Projectiles {
    public class StraightProjectileMovement : ProjectileMovement {
        public StraightProjectileMovement(float speed) : base(speed) { }

        public override void Movement(Transform transform) {
            transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0), Space.Self);
        }
    }
}