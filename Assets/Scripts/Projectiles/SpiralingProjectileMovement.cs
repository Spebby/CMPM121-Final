using UnityEngine;


public class SpiralingProjectileMovement : ProjectileMovement {
    public float Start;

    public SpiralingProjectileMovement(float speed) : base(speed) {
        Start = Time.time;
    }

    public override void Movement(Transform transform) {
        transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0), Space.Self);
        transform.Rotate(
            0, 0, Speed * Mathf.Sqrt(Speed) * Time.deltaTime * 20.0f / (1 + Random.value + Time.time - Start));
    }
}