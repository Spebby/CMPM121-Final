using CMPM.Core;
using CMPM.Relics;
using CMPM.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    public class RelicUI : MonoBehaviour, ITooltipUser {
        Relic _relic;

        public Image icon;
        public GameObject highlight;
        public TextMeshProUGUI label;

        Tooltip _internalTooltip;

        void OnEnable() {
            label.gameObject.SetActive(false);
        }

        public void Init(Relic relic) {
            _relic  = relic;
            label.gameObject.SetActive(false);
            // if a player has relics, this is how you *could* show them
            GameManager.Instance.RelicIconManager.PlaceSprite(_relic.Sprite, icon);
        }

        void Update() {
            highlight.SetActive(_relic.IsActive);
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
    }
}