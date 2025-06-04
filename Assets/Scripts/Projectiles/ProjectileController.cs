
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

        [SerializeField] AudioClip explosionAudioClip;

        // Update is called once per frame
        void Update() {
            Movement.Movement(transform);
        }


        void OnTriggerEnter2D(Collider2D col) {
            if (col.gameObject.CompareTag("projectile")) return;
            if (col.gameObject.layer == 3) Destroy(gameObject);
            if (col.gameObject.CompareTag("unit"))
            {
                EnemyController ec = col.gameObject.GetComponent<EnemyController>();
                if (ec)
                {
                    OnHit!(ec.HP, transform.position);
                    collidedWith++;
                
                    if (explosionAudioClip != null)
                    {
                        AudioSource.PlayClipAtPoint(explosionAudioClip, Camera.main.transform.position, 0.5f);
                    }
                }
            }
            if (col.gameObject.CompareTag("Player")) {
                PlayerController pc = col.gameObject.GetComponent<PlayerController>();
                if (pc)
                {
                    OnHit!(pc.HP, transform.position);
                    collidedWith++;
                }
            }
            if (collidedWith >= HitCap)
            {
                Destroy(gameObject);
            }
           
        }


        public void SetLifetime(float t)
        {
            StartCoroutine(Expire(t));
        }


        IEnumerator Expire(float t) {
            yield return new WaitForSeconds(t);
            Destroy(gameObject);
        }
    }
}
