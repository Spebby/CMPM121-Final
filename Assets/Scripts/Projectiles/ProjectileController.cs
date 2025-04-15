using System;
using System.Collections;
using UnityEngine;


public class ProjectileController : MonoBehaviour {
    public float lifetime;
    public event Action<Hittable, Vector3> OnHit;
    public ProjectileMovement Movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() {
        Movement.Movement(transform);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("projectile")) return;
        if (collision.gameObject.CompareTag("unit")) {
            var ec = collision.gameObject.GetComponent<EnemyController>();
            if (ec) {
                this.OnHit!(ec.Hp, transform.position);
            }
            else {
                var pc = collision.gameObject.GetComponent<PlayerController>();
                if (pc) {
                    this.OnHit!(pc.Hp, transform.position);
                }
            }
        }

        Destroy(gameObject);
    }

    public void SetLifetime(float t) {
        this.StartCoroutine(this.Expire(t));
    }

    IEnumerator Expire(float t) {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
}