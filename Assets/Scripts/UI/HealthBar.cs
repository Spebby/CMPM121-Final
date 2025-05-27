using CMPM.DamageSystem;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.UI {
    public class HealthBar : MonoBehaviour {
        public GameObject slider;

        [FormerlySerializedAs("Hp")] public Hittable HP;

        float _prevRatio;

        void Update() {
            if (HP == null) return;
            float ratio = HP.HP * 1.0f / HP.MaxHP;
            if (!(Mathf.Abs(_prevRatio - ratio) > 0.01f)) return;
            slider.transform.localScale    = new Vector3(ratio, 1, 1);
            slider.transform.localPosition = new Vector3(-(1 - ratio) / 2, 0, 0);
            _prevRatio                     = ratio;
        }

        public void SetHealth(Hittable hp) {
            HP = hp;
            float ratio = hp.HP * 1.0f / hp.MaxHP;

            slider.transform.localScale    = new Vector3(ratio, 1, 1);
            slider.transform.localPosition = new Vector3(-(1 - ratio) / 2, 0, 0);
            _prevRatio                     = ratio;
        }
    }
}