using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.Projectiles {
    public class ProjectileController : MonoBehaviour {
        public float lifetime;
        public int HitCap;
        public int collidedWith;
        public event Action<Hittable, Vector3> OnHit;
        public ProjectileMovement Movement;

        // Update is called once per frame
        void Update() {
            Movement.Movement(transform);
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider.gameObject.CompareTag("projectile")) return;
            if (collider.gameObject.CompareTag("Environment")) Destroy(gameObject);
            if (collider.gameObject.CompareTag("unit")) {
                EnemyController ec = collider.gameObject.GetComponent<EnemyController>();
                if (ec) {
                    OnHit!(ec.HP, transform.position);
                    collidedWith++;
                } else {
                    
                }
            }
            if (collider.gameObject.CompareTag("Player")) {
                PlayerController pc = collider.gameObject.GetComponent<PlayerController>();
                if (pc) {
                    OnHit!(pc.HP, transform.position);
                    collidedWith++;
                }
            }
            if (collidedWith >= HitCap) Destroy(gameObject);
        }

        public void SetLifetime(float t) {
            StartCoroutine(Expire(t));
        }

        IEnumerator Expire(float t) {
            yield return new WaitForSeconds(t);
            Destroy(gameObject);
        }
    }
}