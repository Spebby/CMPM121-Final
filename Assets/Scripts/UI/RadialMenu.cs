using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace CMPM
{
    public class RadialMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject radialMenuEntryPrefab;

        [SerializeField]
        float Radius = 300f;

        [SerializeField]
        List<Texture2D> Icons;

        [SerializeField]
        RawImage TargetIcon;

        List<RadialMenuEntry> Entries;

        public bool isMenuOpen = false;

        void Start()
        {
            Entries = new List<RadialMenuEntry>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Open();
                isMenuOpen = true;
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                Close();
                isMenuOpen = false;
            }
        }

        void AddEntry(string pLabel, Texture2D pIcon, RadialMenuEntry.RadialMenuEntryDelegate pCallback)
        {
            GameObject entry = Instantiate(radialMenuEntryPrefab, transform);

            RadialMenuEntry radialMenuEntry = entry.GetComponent<RadialMenuEntry>();
            radialMenuEntry.SetLabel(pLabel);
            radialMenuEntry.SetIcon(pIcon);
            radialMenuEntry.SetCallBack(pCallback);

            Entries.Add(radialMenuEntry);
        }

        public void Open()
        {
            if (Entries.Count > 0) return; // Prevent reopening if already open

            for (int i = 0; i < 5; i++)
            {
                AddEntry("Button " + i.ToString(), Icons[i], SetTargetIcon);
            }
            Rearrange();
        }

        public void Close()
        {
            if (Entries.Count == 0) return; // Prevent closing if already closed

            for (int i = 0; i < Entries.Count; i++)
            {
                RectTransform rect = Entries[i].GetComponent<RectTransform>();
                GameObject entry = Entries[i].gameObject;

                rect.DOAnchorPos(Vector3.zero, .3f).SetEase(Ease.OutQuad).onComplete =
                    delegate ()
                    {
                        Destroy(entry);
                    };
            }
            Entries.Clear();
        }

        void Rearrange()
        {
            float radiansOfSeparation = (Mathf.PI * 2) / Entries.Count;
            for (int i = 0; i < Entries.Count; i++)
            {
                float x = Mathf.Sin(radiansOfSeparation * i) * Radius;
                float y = Mathf.Cos(radiansOfSeparation * i) * Radius;

                RectTransform rect = Entries[i].GetComponent<RectTransform>();

                rect.localScale = Vector3.zero;
                rect.DOScale(Vector3.one, .3f).SetEase(Ease.OutQuad).SetDelay(0.05f * i);
                rect.DOAnchorPos(new Vector3(x, y, 0f), .3f).SetEase(Ease.OutQuad).SetDelay(0.05f * i);
            }
        }

        void SetTargetIcon(RadialMenuEntry pEntry)
        {
            TargetIcon.texture = pEntry.GetIcon();
        }
    }
}