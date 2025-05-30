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
        RawImage Icon;

        RectTransform Rect;
        RadialMenuEntryDelegate Callback;

        public void Start()
        {
            Rect = Icon.GetComponent<RectTransform>();
        }

        public void SetLabel(string pText)
        {
            Label.text = pText;
        }

        public void SetIcon(Texture2D pIcon)
        {
            Icon.texture = pIcon;
        }

        public Texture GetIcon()
        {
            return (Icon.texture);
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
            Rect.DOComplete();
            Rect.DOScale(Vector3.one * 1.5f, .3f).SetEase(Ease.OutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Rect.DOComplete();
            Rect.DOScale(Vector3.one, .3f).SetEase(Ease.OutQuad);
        }
    }
}
