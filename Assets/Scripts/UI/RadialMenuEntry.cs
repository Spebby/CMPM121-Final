using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace CMPM.UI {
    public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        public delegate void RadialMenuEntryDelegate(RadialMenuEntry pEntry);
        public bool IsHovered { get; private set; }
        
        
        [SerializeField] TextMeshProUGUI Label;
        [SerializeField] public GameObject IconObject; // Changed from RawImage to GameObject
        public GameObject spellUI;
        [SerializeField, Range(0.05f, 0.5f)] float tweenTimer = 0.1f;
        
        RectTransform _rect;
        RadialMenuEntryDelegate _callback;
        RadialMenuEntryDelegate _doDrop;

        public void Start() {
            if (IconObject) {
                _rect = IconObject.GetComponent<RectTransform>();
            }
        }

        public void SetLabel(string pText) {
            Label.text = pText;
        }

        public void SetIcon(Sprite pIcon) {
            if (!IconObject) return;
            Image image = IconObject.GetComponent<Image>();
            if (image) {
                image.sprite = pIcon;
            }
        }

        public Sprite GetIcon() {
            if (!IconObject) return null;
            Image image = IconObject.GetComponent<Image>();
            return image ? image.sprite : null;
        }

        public void SetCallBack(RadialMenuEntryDelegate pCallback) {
            _callback = pCallback;
        }
        
        public void SetDropCallBack(RadialMenuEntryDelegate callback) {
            _doDrop = callback;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                _callback?.Invoke(this);
            } else if (eventData.button == PointerEventData.InputButton.Right) {
                _doDrop?.Invoke(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!_rect) return;
            IsHovered = true;
            _rect.DOComplete();
            _rect.DOScale(Vector3.one * 1.5f, tweenTimer)
                 .SetEase(Ease.OutQuad)
                 .SetLink(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!_rect) return;
            IsHovered = false;
            _rect.DOComplete();
            _rect.DOScale(Vector3.one, tweenTimer)
                 .SetEase(Ease.OutQuad)
                 .SetLink(gameObject);
        }
    }
}