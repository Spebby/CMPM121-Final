using CMPM.Core;
using CMPM.Relics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    // TODO: make this a tooltip user
    public class ClassSelector : MonoBehaviour {
        PlayerController.PlayerClass _class;

        public Image icon;
        public TextMeshProUGUI label;

        Button _button;
        Tooltip _internalTooltip;

        public void Start() {
            _button = GetComponent<Button>();
        }
        
        public void Init(PlayerController.PlayerClass c) {
            _class     = c;
            label.text = c.Class.ToString();
            GameManager.Instance.PlayerSpriteManager.PlaceSprite(c.Sprite, icon);
        }

        /*
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
        */
    }
}