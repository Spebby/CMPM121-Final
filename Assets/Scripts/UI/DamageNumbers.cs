using UnityEngine;
using UnityEngine.Serialization;


public class DamageNumbers : MonoBehaviour {
    [FormerlySerializedAs("DamageNumber")] public GameObject damageNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        EventBus.Instance.OnDamage += this.OnDamage;
    }

    // Update is called once per frame
    void Update() { }

    void OnDamage(Vector3 where, Damage dmg, Hittable target) {
        GameObject newDmgNr = Instantiate(damageNumber, where, Quaternion.identity);
        Vector3    dmgPos   = where + new Vector3(0, 0, -2);
        newDmgNr.GetComponent<AnimateDamage>().Setup(dmg.Amount.ToString(), dmgPos, dmgPos + new Vector3(0, 3, 0), 10,
                                                     2, Color.magenta, Color.white, 1.5f);
    }
}