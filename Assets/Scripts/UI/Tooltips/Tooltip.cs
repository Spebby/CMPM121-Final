using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace CMPM.UI.Tooltips {
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [FormerlySerializedAs("Body")] [Header("Components")] 
        public GameObject body;
        public TMP_Text title;
        public TMP_Text description;
        
        void OnDestroy() {
            Destroy(gameObject);
        }

        public bool IsHovering { get;  private set; }
        // public bool IsTriggerHovered { get;  private set; }

        protected virtual void Show(Vector3 pos, string label, string desc) {
            title.text       = label;
            title.enableAutoSizing = false;
            title.fontSize   = 30f;
            description.text = desc;
            description.enableAutoSizing = false;
            description.fontSize = 20f;

            Vector2 offset = new(10f, -10f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                body.transform.parent as RectTransform,
                pos + (Vector3)offset,
                null, // assumes Screen Space - Overlay canvas
                out Vector2 anchoredPosition
            );

            RectTransform rect = body.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;

            gameObject.SetActive(true);
            transform.position = pos;
            ClampToScreen(body, rect);
        }

        public void OnTriggerHoverChanged(bool hovering, string label, string desc) {
            // IsTriggerHovered = hovering;
            if (hovering) {
                Show(Input.mousePosition, label, desc);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData) {
            IsHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            IsHovering = false;
        }

        static void ClampToScreen(GameObject body, RectTransform rect) {
            float screenWidth  = Screen.width;
            float screenHeight = Screen.height;
            float padding = 10f;

            Vector3 pos = body.transform.position;

            // somehow this works... with class selection at least
            pos.x = Mathf.Clamp(pos.x, (screenWidth / -2) + padding, (screenWidth / 2) - padding);
            pos.y = Mathf.Clamp(pos.y, (screenHeight / -2) + padding, (screenHeight / 2) - padding);


            body.transform.position = pos;
        } 
    }
}