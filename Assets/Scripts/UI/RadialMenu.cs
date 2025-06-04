using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using CMPM.Spells;
using CMPM.Core;

namespace CMPM.UI
{
    public class RadialMenu : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject radialMenuEntryPrefab;
        [SerializeField] private float radius = 150f;
        [SerializeField] private float singleSpellOffset = 50f;
        [SerializeField] public GameObject targetIcon;

        private List<RadialMenuEntry> entries = new List<RadialMenuEntry>();
        private bool isInitialized = false;

        public bool IsMenuOpen { get; private set; }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (isInitialized) return;
            
            if (radialMenuEntryPrefab == null)
            {
                Debug.LogError("[RadialMenu] Radial Menu Entry Prefab is not assigned!");
                return;
            }

            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized) return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TryOpenMenu();
            }
            else if (Input.GetKeyUp(KeyCode.Tab) && IsMenuOpen)
            {
                Close();
            }
        }

        private void TryOpenMenu()
        {
            if (IsMenuOpen || GameManager.Instance == null || 
                GameManager.Instance.State != GameManager.GameState.INWAVE)
                return;

            var playerController = GameManager.Instance.PlayerController;
            var spells = playerController.GetSpells();

            Open(spells);
        }

    private void Open(Spell[] spells)
    {
        // Clear existing entries
        if (entries.Count > 0)
        {
            foreach (var entry in entries)
            {
                if (entry != null) Destroy(entry.gameObject);
            }
            entries.Clear();
        }

        for (int i = 0; i < spells.Length; i++)
        {
            if (spells[i] == null) continue;
            CreateMenuEntry(spells[i].GetName(), spells[i], i);
        }

        if (entries.Count > 0)
        {
            Rearrange();
            IsMenuOpen = true;
            Time.timeScale = 0.3f; 
        }
    }

    private void CreateMenuEntry(string label, Spell spell, int spellIndex)
    {
        var entry = Instantiate(radialMenuEntryPrefab, transform);
        if (entry == null) return;

        var menuEntry = entry.GetComponent<RadialMenuEntry>();
        if (menuEntry == null)
        {
            Destroy(entry);
            return;
        }

        menuEntry.SetLabel(label);
    
        // Set the spell icon similar to SpellUI's SetSpell method
        if (spell != null)
        {
            if (GameManager.Instance?.SpellIconManager != null)
            {
                // Get the Image component from the RadialMenuEntry's icon GameObject
                var iconImage = menuEntry.IconObject?.GetComponent<Image>();
                if (iconImage != null)
                {   
                    GameManager.Instance.SpellIconManager.PlaceSprite(spell.GetIcon(), iconImage);
                }
            }
        }

        menuEntry.SetCallBack((_) => OnSpellSelected(spellIndex));
        entries.Add(menuEntry);
    }   

        private void Rearrange()
        {
            if (entries.Count == 0) return;

            if (entries.Count == 1)
            {
                var rect = entries[0].GetComponent<RectTransform>();
                rect.localScale = Vector3.zero;
                rect.DOAnchorPos(new Vector3(0, singleSpellOffset, 0), 0.3f)
                    .SetEase(Ease.OutBack);
                rect.DOScale(Vector3.one, 0.3f)
                    .SetEase(Ease.OutBack);
                return;
            }

            float radiansOfSeparation = (Mathf.PI * 2) / entries.Count;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] == null) continue;

                var rect = entries[i].GetComponent<RectTransform>();
                if (rect == null) continue;

                float x = Mathf.Sin(radiansOfSeparation * i) * radius;
                float y = Mathf.Cos(radiansOfSeparation * i) * radius;

                rect.localScale = Vector3.zero;
                rect.DOAnchorPos(new Vector3(x, y, 0f), 0.3f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.05f * i);
                rect.DOScale(Vector3.one, 0.3f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.05f * i);
            }
        }

        public void Close()
        {
            if (!IsMenuOpen) return;

            foreach (var entry in entries)
            {
                if (entry == null) continue;

                var rect = entry.GetComponent<RectTransform>();
                if (rect == null) continue;

                rect.DOAnchorPos(Vector3.zero, 0.2f)
                    .SetEase(Ease.InQuad)
                    .onComplete += () => Destroy(entry.gameObject);
            }

            entries.Clear();
            IsMenuOpen = false;
            Time.timeScale = 1f;
        }

        private void OnSpellSelected(int spellIndex)
        {
            var playerController = GameManager.Instance.PlayerController;
            if (playerController == null) return;

            playerController.SwitchSpell(spellIndex);

            if (targetIcon != null)
            {
                var spells = playerController.GetSpells();
                if (spellIndex < spells.Length && spells[spellIndex] != null)
                {
                    var sprite = GameManager.Instance.SpellIconManager?.Get(spells[spellIndex].GetIcon());
                }
            }
            
            Close();
        }
    }
}