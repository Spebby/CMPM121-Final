using System;
using CMPM.Core;
using CMPM.Spells;
using CMPM.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    public class SpellUI : MonoBehaviour, ITooltipUser {
        public GameObject icon;
        public RectTransform cooldown;
        public TextMeshProUGUI manacost;
        public TextMeshProUGUI damage;
        public GameObject highlight;
        Spell _spell;
        float _lastTextUpdate;
        const float UPDATE_DELAY = 1;
        public Button dropButton;

        Tooltip _internalTooltip;

        void OnDestroy() {
            if (_internalTooltip) Destroy(_internalTooltip);
        }

        void OnDisable() {
            if (_internalTooltip) Destroy(_internalTooltip);
        }

        void Start() {
            _lastTextUpdate = 0;
            if (highlight) highlight.SetActive(false);
            manacost.text = "";
            damage.text   = "";
            dropButton?.onClick.AddListener(() => {
                GameManager.Instance.PlayerController.DropSpell(transform.GetSiblingIndex());
            });
        }

        public void SetSpell(Spell spell) {
            _spell = spell;
            if (spell != null) {
                GameManager.Instance.SpellIconManager.PlaceSprite(spell.GetIcon(), icon.GetComponent<Image>());
            } else {
                icon.GetComponent<Image>().sprite = null;
            }
        }

        public bool IsEmpty() {
            return _spell == null;
        }

        // Update is called once per frame
        void Update() {
            if (_spell == null) return;
            if (Time.time > _lastTextUpdate + UPDATE_DELAY) {
                manacost.text = _spell.GetManaCost().ToString();
                damage.text = _spell.GetDamage().ToString();
                _lastTextUpdate = Time.time;
            }

            if (!cooldown) return;
            float sinceLast = Time.time - _spell.LastCast;
            float ratio = sinceLast > _spell.GetCooldown() ? 0 : 1 - sinceLast / _spell.GetCooldown();
            cooldown.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 48 * ratio);

            if (_spell == GameManager.Instance.PlayerController.GetActiveSpell())
            {
                highlight.SetActive(true);
            } else {
                highlight.SetActive(false);
            }
        }
        
        public void ShowTooltip(Tooltip tooltip) {
            if (_spell == null) return;
            if (_internalTooltip) {
                Destroy(_internalTooltip.gameObject);
            }
            
            _internalTooltip = Instantiate(tooltip, GameObject.FindWithTag("Canvas").transform, true);
            _internalTooltip.OnTriggerHoverChanged(true, _spell.GetName(), _spell.GetDescription());
        }
        
        public void HideTooltip() {
            if (!_internalTooltip) return;
            Destroy(_internalTooltip.gameObject);
            _internalTooltip = null;
        }
        
        public bool IsHovering() => _internalTooltip?.IsHovering ?? false;
    }
}