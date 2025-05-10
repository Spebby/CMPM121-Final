using CMPM.Core;
using CMPM.Spells;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace CMPM.UI {
    public class SpellUI : MonoBehaviour {
        public GameObject icon;
        public RectTransform cooldown;
        public TextMeshProUGUI manacost;
        public TextMeshProUGUI damage;
        public GameObject highlight;
        Spell _spell;
        float _lastTextUpdate;
        const float UPDATE_DELAY = 1;
        [FormerlySerializedAs("dropbutton")] public GameObject dropButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            _lastTextUpdate = 0;
            if (highlight) highlight.SetActive(false);
            manacost.text = "";
            damage.text = "";
        }
        
        public void SetSpell(Spell spell) {
            _spell = spell;
            if (spell != null) {
                GameManager.Instance.SpellIconManager.PlaceSprite(spell.GetIcon(), icon.GetComponent<Image>());
            } else {
                icon.GetComponent<Image>().sprite = null;
            }
        }
        
        public bool IsEmpty() => _spell == null;

        // Update is called once per frame
        void Update() {
            if (_spell == null) return;
            if (Time.time > _lastTextUpdate + UPDATE_DELAY) {
                manacost.text   = _spell.GetManaCost().ToString();
                damage.text     = _spell.GetDamage().ToString();
                _lastTextUpdate = Time.time;
            }

            if (!cooldown) return;
            float sinceLast = Time.time - _spell.LastCast;
            float ratio     = sinceLast > _spell.GetCooldown() ? 0 : 1 - sinceLast / _spell.GetCooldown();
            cooldown.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 48 * ratio);
        }
    }
}