using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.UI;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.Movement {
    public class EnemyController : MonoBehaviour {
        public Transform target;
        public int speed;
        public Hittable HP;
        [FormerlySerializedAs("healthui")] public HealthBar healthUI;
        public bool dead;

        [FormerlySerializedAs("last_attack")] public float lastAttack;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            target     =  GameManager.INSTANCE.Player.transform;
            HP.OnDeath += Die;
            healthUI.SetHealth(HP);
        }

        // Update is called once per frame
        void Update() {
            Vector3 direction = target.position - transform.position;
            if (direction.magnitude < 2f) {
                DoAttack();
            } else {
                GetComponent<Unit>().movement = direction.normalized * speed;
            }
        }

        void DoAttack() {
            if (!(lastAttack + 2 < Time.time)) return;
            lastAttack = Time.time;
            target.gameObject.GetComponent<PlayerController>().Hp.Damage(new Damage(5, Damage.Type.PHYSICAL));
        }

        void Die() {
            if (dead) return;
            dead = true;
            GameManager.INSTANCE.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}