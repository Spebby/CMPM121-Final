using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.UI {
    public class HealthBar : MonoBehaviour {
        public GameObject slider;

        public Hittable Hp;

        float _prevRatio;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() { }

        // Update is called once per frame
        void Update() {
            if (Hp == null) return;
            float ratio = Hp.Hp * 1.0f / Hp.MaxHp;
            if (!(Mathf.Abs(_prevRatio - ratio) > 0.01f)) return;
            slider.transform.localScale    = new Vector3(ratio, 1, 1);
            slider.transform.localPosition = new Vector3(-(1 - ratio) / 2, 0, 0);
            _prevRatio                     = ratio;
        }

        public void SetHealth(Hittable hp) {
            Hp = hp;
            float ratio = hp.Hp * 1.0f / hp.MaxHp;

            slider.transform.localScale    = new Vector3(ratio, 1, 1);
            slider.transform.localPosition = new Vector3(-(1 - ratio) / 2, 0, 0);
            _prevRatio                     = ratio;
        }
    }
}