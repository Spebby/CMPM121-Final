using UnityEngine;


namespace CMPM.Projectiles {
    public class SineProjectileMovement : ProjectileMovement {
        float _travelledDistance;

        public SineProjectileMovement(float speed) : base(speed) { }

        public override void Movement(Transform transform) {
            float dt = Time.deltaTime;
            float dx = Speed * dt;
            _travelledDistance += dx;

            float frequency = Speed * 0.25f;
            float amplitude = 1.0f / (0.5f + Mathf.Sqrt(Speed));
            float dy        = Mathf.Sin(_travelledDistance * frequency) * amplitude;

            transform.Translate(new Vector3(dx, dy, 0), Space.Self);
        }
    }
}