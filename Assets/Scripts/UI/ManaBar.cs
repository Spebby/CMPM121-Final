using CMPM.Spells;
using UnityEngine;


namespace CMPM.UI {
    public class ManaBar : MonoBehaviour {
        [SerializeField] GameObject slider;
        SpellCaster _caster;

        float _prevRatio;

        void Update() {
            if (_caster == null) return;
            float ratio = _caster.Mana * 1.0f / _caster.MaxMana;
            if (!(Mathf.Abs(_prevRatio - ratio) > 0.01f)) return;
            
            slider.transform.localScale    = new Vector3(ratio, 1, 1);
            slider.transform.localPosition = new Vector3(-(1 - ratio) / 2, 0, 0);
            _prevRatio                     = ratio;
        }

        public void SetSpellCaster(SpellCaster sc) {
            _caster    = sc;
            _prevRatio = 0;
        }
    }
}