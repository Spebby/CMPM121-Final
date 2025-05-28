using CMPM.Core;
using CMPM.Relics;
using CMPM.UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    public class RelicIcon : MonoBehaviour, ITooltipUser {
        RelicData _relic;
        [SerializeField] Image icon;
        Tooltip _internalTooltip;

        void OnDestroy() {
            if (_internalTooltip) Destroy(_internalTooltip);
        }

        void OnDisable() {
            if (_internalTooltip) Destroy(_internalTooltip);
        }

        public void Init(RelicData relic) {
            _relic  = relic;
            // if a player has relics, this is how you *could* show them
            GameManager.Instance.RelicIconManager.PlaceSprite(_relic.Sprite, icon);
        }

        public void ShowTooltip(Tooltip tooltip) {
            if (_internalTooltip) {
                Destroy(_internalTooltip.gameObject);
            }
            
            _internalTooltip = Instantiate(tooltip, GameObject.FindWithTag("Canvas").transform, true);
            _internalTooltip.OnTriggerHoverChanged(true, _relic.Name, _relic.GetFullDescription());
        }
        
        public void HideTooltip() {
            if (!_internalTooltip) return;
            Destroy(_internalTooltip.gameObject);
            _internalTooltip = null;
        }
        
        public bool IsHovering() => _internalTooltip?.IsHovering ?? false;
    }
}