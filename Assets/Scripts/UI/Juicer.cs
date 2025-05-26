using CMPM.Core;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.UI {
    public class Juicer : MonoBehaviour {
        [SerializeField] GameObject damageNumber;
        [SerializeField] GameObject healNumber;
        
        void Start() {
            EventBus.Instance.OnDamage += OnDamage;
            EventBus.Instance.OnHeal   += OnHeal;
        }

        void OnHeal(Vector3 where, int amount, Hittable target) {
            GameObject newNbr = Instantiate(damageNumber, where, Quaternion.identity);
            Vector3 pos = where + new Vector3(0, 0, -2);
            newNbr.GetComponent<AnimateNumber>().Setup(amount.ToString(), pos, pos + new Vector3(0, 3, 0), 10,
                                                          2, Color.green, Color.white, 1.5f);
        }

        void OnDamage(Vector3 where, Damage dmg, Hittable target) {
            GameObject newNbr = Instantiate(damageNumber, where, Quaternion.identity);
            Vector3    pos   = where + new Vector3(0, 0, -2);
            newNbr.GetComponent<AnimateNumber>().Setup(dmg.Amount.ToString(), pos, pos + new Vector3(0, 3, 0),
                                                         10,
                                                         2, Color.magenta, Color.white, 1.5f);
        }
    }
}