using UnityEngine;
using UnityEngine.Serialization;


public class EnemyController : MonoBehaviour {
    public Transform target;
    public int speed;
    public Hittable Hp;
    public HealthBar healthui;
    public bool dead;

    [FormerlySerializedAs("last_attack")] public float lastAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        target     =  GameManager.Instance.Player.transform;
        Hp.OnDeath += this.Die;
        healthui.SetHealth(Hp);
    }

    // Update is called once per frame
    void Update() {
        Vector3 direction = target.position - transform.position;
        if (direction.magnitude < 2f) {
            this.DoAttack();
        }
        else {
            this.GetComponent<Unit>().movement = direction.normalized * speed;
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
        GameManager.Instance.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }
}