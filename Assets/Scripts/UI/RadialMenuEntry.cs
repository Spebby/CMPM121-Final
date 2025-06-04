using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace CMPM
{
    public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void RadialMenuEntryDelegate(RadialMenuEntry pEntry);

        [SerializeField]
        TextMeshProUGUI Label;

        [SerializeField]
        public GameObject IconObject; // Changed from RawImage to GameObject

        RectTransform Rect;
        RadialMenuEntryDelegate Callback;

        public void Start()
        {
            if (IconObject != null)
            {
                Rect = IconObject.GetComponent<RectTransform>();
            }
        }

        public void SetLabel(string pText)
        {
            Label.text = pText;
        }

        public void SetIcon(Sprite pIcon)
        {
            if (IconObject != null)
            {
                var image = IconObject.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = pIcon;
                }
            }
        }

        public Sprite GetIcon()
        {
            if (IconObject != null)
            {
                var image = IconObject.GetComponent<Image>();
                if (image != null)
                {
                    return image.sprite;
                }
            }
            return null;
        }

        public void SetCallBack(RadialMenuEntryDelegate pCallback)
        {
            Callback = pCallback;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Callback?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Rect != null)
            {
                Rect.DOComplete();
                Rect.DOScale(Vector3.one * 1.5f, .3f).SetEase(Ease.OutQuad);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Rect != null)
            {
                Rect.DOComplete();
                Rect.DOScale(Vector3.one, .3f).SetEase(Ease.OutQuad);
            }
        }
    }
}