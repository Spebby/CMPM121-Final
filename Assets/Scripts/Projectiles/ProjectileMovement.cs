using UnityEngine;


public class ProjectileMovement {
    public float Speed;

    public ProjectileMovement(float speed) {
        Speed = speed;
    }

    public virtual void Movement(Transform transform) { }
}