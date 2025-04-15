using UnityEngine;


public class HomingProjectileMovement : ProjectileMovement {
    float _angle;
    readonly float _turnRate;

    public HomingProjectileMovement(float speed) : base(speed) {
        _angle    = float.NaN;
        _turnRate = 0.25f;
    }

    public override void Movement(Transform transform) {
        if (float.IsNaN(_angle)) {
            Vector3 direction = transform.rotation * new Vector3(1, 0, 0);
            _angle = Mathf.Atan2(direction.y, direction.x);
        }

        GameObject closest = GameManager.Instance.GetClosestEnemy(transform.position);
        if (!closest) {
            Vector3 direction = transform.rotation * new Vector3(1, 0, 0);
            _angle = Mathf.Atan2(direction.y, direction.x);
            transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0), Space.Self);
        }
        else {
            Vector3 newDirection = (closest.transform.position - transform.position).normalized;
            float   newAngle     = Mathf.Atan2(newDirection.y, newDirection.x);
            if (Mathf.Abs(_angle - newAngle) > Mathf.Epsilon) {
                float da = newAngle - _angle;
                if (da > Mathf.PI) {
                    da -= 2 * Mathf.PI;
                }

                if (da < -Mathf.PI) {
                    da += 2 * Mathf.PI;
                }

                _angle += Mathf.Clamp(da, -_turnRate * Mathf.Deg2Rad, _turnRate * Mathf.Deg2Rad);
            }

            var direction = new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle), 0);
            transform.Translate(direction.normalized * Speed * Time.deltaTime, Space.World);
        }
    }
}