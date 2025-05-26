using CMPM.Core;
using CMPM.Relics;
using CMPM.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    public class RelicUI : MonoBehaviour, ITooltipUser {
        Relic _relic;
        Tooltip _internalTooltip;

        [SerializeField] Image icon;
        [SerializeField] GameObject highlight;
        [SerializeField] TextMeshProUGUI label;

        void OnDestroy() {
            if (_internalTooltip) Destroy(_internalTooltip);
        }
        
        void OnEnable() {
            label.gameObject.SetActive(false);
        }

        void OnDisable() {
            if (_internalTooltip) Destroy(_internalTooltip);
        }

        public void Init(Relic relic) {
            _relic  = relic;
            label.gameObject.SetActive(false);
            // if a player has relics, this is how you *could* show them
            GameManager.Instance.RelicIconManager.PlaceSprite(_relic.Sprite, icon);
        }

        void Update() {
            if (_relic.ShouldHighlight) {
                highlight.SetActive(_relic.IsActive);
            }
        }

        public void ShowTooltip(Tooltip tooltip) {
            if (_internalTooltip) {
                Destroy(_internalTooltip.gameObject);
            }
            
            _internalTooltip = Instantiate(tooltip, GameObject.FindWithTag("Canvas").transform, true);
            _internalTooltip.OnTriggerHoverChanged(true, _relic.Name, _relic.Description);
        }
        
        public void HideTooltip() {
            if (!_internalTooltip) return;
            Destroy(_internalTooltip.gameObject);
            _internalTooltip = null;
        }

        public bool IsHovering() => _internalTooltip?.IsHovering ?? false;
    }
}