using CMPM.Core;
using CMPM.Relics;
using TMPro;
using Unity.VisualScripting;
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

        public void ShowTooltip(Tooltip tooltip, Vector3 pos) {
            if (_internalTooltip) {
                Destroy(_internalTooltip.gameObject);
            }
            
            _internalTooltip = Instantiate(tooltip);
            bool hasPrecondition = string.IsNullOrEmpty(_relic.PreconditionDescription);
            bool hasEffect = string.IsNullOrEmpty(_relic.EffectDescription);
            string body = $"{_relic.Description}{(hasEffect || hasPrecondition ? '\n' : "")}{(hasPrecondition ? '\n' : $"\n{_relic.PreconditionType.ToString()}: {_relic.PreconditionDescription}")}{(hasEffect ? '\n' : $"\n{_relic.EffectType.Description()}: {_relic.EffectDescription}")}";
            _internalTooltip.OnTriggerHoverChanged(true, _relic.Name, body);
        }
        
        public void HideTooltip() {
            _internalTooltip.OnTriggerHoverChanged(false, null, null);
            Destroy(_internalTooltip.gameObject);
            _internalTooltip = null;
        }
    }
}