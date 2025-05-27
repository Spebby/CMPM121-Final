using System;
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
        public bool IsTriggerHovered { get;  private set; }

        protected virtual void Show(Vector3 pos, string label, string desc) {
            title.text       = label;
            description.text = desc;

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
            IsTriggerHovered = hovering;
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
            Vector3[]     corners     = new Vector3[4];
            rect.GetWorldCorners(corners);

            float screenWidth  = Screen.width;
            float screenHeight = Screen.height;

            Vector3 pos = body.transform.position;

            float width  = corners[2].x - corners[0].x;
            float height = corners[2].y - corners[0].y;

            // Clamp right and top edges
            if (pos.x + width > screenWidth)   pos.x   = screenWidth - width;
            if (pos.y + height > screenHeight) pos.y = screenHeight - height;

            // Clamp left and bottom edges
            if (pos.x < 0) pos.x = 0;
            if (pos.y < 0) pos.y = 0;

            body.transform.position = pos;
        } 
    }
}