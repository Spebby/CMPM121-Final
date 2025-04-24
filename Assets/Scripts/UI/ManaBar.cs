using CMPM.Spells;
using UnityEngine;


namespace CMPM.UI {
    public class ManaBar : MonoBehaviour {
        public GameObject slider;

        public SpellCaster Sc;

        float _prevRatio;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() { }

        // Update is called once per frame
        void Update() {
            if (Sc == null) return;
            float ratio = Sc.Mana * 1.0f / Sc.MaxMana;
            if (Mathf.Abs(_prevRatio - ratio) > 0.01f) {
                slider.transform.localScale    = new Vector3(ratio, 1, 1);
                slider.transform.localPosition = new Vector3(-(1 - ratio) / 2, 0, 0);
                _prevRatio                     = ratio;
            }
        }

        public void SetSpellCaster(SpellCaster sc) {
            Sc         = sc;
            _prevRatio = 0;
        }
    }
}