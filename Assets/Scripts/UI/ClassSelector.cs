using CMPM.Core;
using CMPM.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    // TODO: make this a tooltip user
    public class ClassSelector : MonoBehaviour, ITooltipUser {
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
        
        public void ShowTooltip(Tooltip tooltip) {
            if (_internalTooltip) {
                Destroy(_internalTooltip.gameObject);
            }
            
            _internalTooltip = Instantiate(tooltip, GameObject.FindWithTag("Canvas").transform, true);
            _internalTooltip.OnTriggerHoverChanged(true, _class.Class.ToString(), _class.Description);
        }
        
        public void HideTooltip() {
            if (!_internalTooltip) return;
            Destroy(_internalTooltip.gameObject);
            _internalTooltip = null;
        }
    }
}