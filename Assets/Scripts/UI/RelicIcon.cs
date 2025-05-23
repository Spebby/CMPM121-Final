using CMPM.Core;
using CMPM.Relics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    public class RelicIcon : MonoBehaviour, ITooltipUser {
        RelicData _relic;

        public Image icon;

        Tooltip _internalTooltip;

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
            bool hasPrecondition = !string.IsNullOrEmpty(_relic.Precondition.Description);
            bool hasEffect = !string.IsNullOrEmpty(_relic.Effect.Description);
            string body = $"{_relic.Description}{(hasEffect || hasPrecondition ? '\n' : "")}{(hasPrecondition ? '\n' : $"\n{_relic.Precondition.Type.ToString()}: {_relic.Precondition.Description}")}{(hasEffect ? '\n' : $"\n{_relic.Effect.Type.Description()}: {_relic.Effect.Description}")}";
            _internalTooltip.OnTriggerHoverChanged(true, _relic.Name, body);
        }
        
        public void HideTooltip() {
            Destroy(_internalTooltip.gameObject);
            _internalTooltip = null;
        }
    }
}