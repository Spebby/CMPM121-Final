using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using CMPM.Spells;
using CMPM.Core;


namespace CMPM.UI {
    public class RadialMenu : MonoBehaviour {
        [Header("Settings")] [SerializeField] GameObject radialMenuEntryPrefab;
        [SerializeField] float radius = 150f;
        [SerializeField] float singleSpellOffset = 50f;
        [SerializeField] GameObject targetIcon;
        [SerializeField] float tweenInDuration = 0.1f;
        [SerializeField] float tweenOutDuration = 0.15f;
        [SerializeField] float tweenDelay = 0.05f;

        readonly List<RadialMenuEntry> _entries = new();
        bool _isInitialized;

        public bool IsMenuOpen { get; private set; }

        void Start() {
            Initialize();
        }

        void Initialize() {
            if (_isInitialized) return;

            if (radialMenuEntryPrefab == null) {
                Debug.LogError("[RadialMenu] Radial Menu Entry Prefab is not assigned!");
                return;
            }

            _isInitialized = true;
        }

        void Update() {
            if (!_isInitialized) return;

            if (Input.GetKeyDown(KeyCode.Tab)) {
                TryOpenMenu();
            } else if (Input.GetKeyUp(KeyCode.Tab) && IsMenuOpen) {
                Close();
            }
        }

        void TryOpenMenu() {
            if (IsMenuOpen || GameManager.Instance == null ||
                GameManager.Instance.State != GameManager.GameState.INWAVE)
                return;

            PlayerController playerController = GameManager.Instance.PlayerController;
            Spell[]          spells           = playerController.GetSpells();

            Open(spells);
        }

        void Open(Spell[] spells) {
            foreach (RadialMenuEntry entry in _entries) {
                if (entry) Destroy(entry.gameObject);
            }
            _entries.Clear();

            for (int i = 0; i < spells.Length; i++) {
                if (spells[i] == null) continue;
                CreateMenuEntry(spells[i].GetName(), spells[i], i);
            }

            if (_entries.Count <= 0) return;
            Rearrange();
            IsMenuOpen     = true;
            Time.timeScale = 0.3f;
        }

        void CreateMenuEntry(string label, Spell spell, int spellIndex) {
            GameObject entry = Instantiate(radialMenuEntryPrefab, transform);
            if (!entry) return;

            RadialMenuEntry menuEntry = entry.GetComponent<RadialMenuEntry>();
            if (!menuEntry) {
                Destroy(entry);
                return;
            }

            menuEntry.SetLabel(label);

            // Set the spell icon similar to SpellUI SetSpell method
            if (spell != null) {
                if (GameManager.Instance?.SpellIconManager != null) {
                    // Get the Image component from the RadialMenuEntry's icon GameObject
                    // Image iconImage = menuEntry.IconObject?.GetComponent<Image>();
                    // if (iconImage) {
                    //     GameManager.Instance.SpellIconManager.PlaceSprite(spell.GetIcon(), iconImage);
                    // }
                    menuEntry._SpellUI.GetComponent<SpellUI>().SetSpell(spell);
                }
            }

            menuEntry.SetCallBack(_ => {
                OnSpellSelected(spellIndex);
                Close(noSelect: true);
            });
            _entries.Add(menuEntry);
        }

        void Rearrange() {
            switch (_entries.Count) {
                case 0:
                    return;
                case 1: {
                    RectTransform rect = _entries[0].GetComponent<RectTransform>();
                    rect.localScale = Vector3.zero;
                    rect.DOAnchorPos(new Vector3(0, singleSpellOffset, 0), 0.3f)
                        .SetEase(Ease.OutBack);
                    rect.DOScale(Vector3.one, 0.3f)
                        .SetEase(Ease.OutBack);
                    return;
                }
            }

            float radiansOfSeparation = Mathf.PI * 2 / _entries.Count;
            for (int i = 0; i < _entries.Count; i++) {
                if (!_entries[i]) continue;

                RectTransform rect = _entries[i].GetComponent<RectTransform>();
                if (!rect) continue;

                float x = Mathf.Sin(radiansOfSeparation * i) * radius;
                float y = Mathf.Cos(radiansOfSeparation * i) * radius;

                rect.localScale = Vector3.zero;
                
                DOTween.Sequence()
                       .Join(rect.DOAnchorPos(new Vector3(x, y), tweenInDuration).SetEase(Ease.OutBack).SetDelay(tweenDelay * i))
                       .Join(rect.DOScale(Vector3.one, tweenInDuration).SetEase(Ease.OutBack).SetDelay(tweenDelay * i))
                       .SetLink(gameObject);
            }
        }

        public void Close(bool noSelect = false) {
            if (!IsMenuOpen) return;

            if (!noSelect) {
                for (int i = 0; i < _entries.Count; i++) {
                    if (!_entries[i].IsHovered) continue;
                    OnSpellSelected(i);
                    break;
                }
            }

            foreach (RadialMenuEntry entry in _entries) {
                if (!entry) continue;

                RectTransform rect = entry.GetComponent<RectTransform>();
                if (!rect) continue;

                DOTween.Sequence()
                   .Join(rect.DOScale(Vector3.one, tweenOutDuration).SetEase(Ease.OutBack))
                   .Join(rect.DOAnchorPos(Vector3.zero, tweenOutDuration).SetEase(Ease.InQuad))
                   .OnComplete(() => {
                        rect.DOKill();
                        Destroy(entry.gameObject);
                   })
                  .SetLink(gameObject);
            }

            _entries.Clear();
            IsMenuOpen     = false;
            Time.timeScale = 1f;
        }

        void OnSpellSelected(int spellIndex) {
            PlayerController playerController = GameManager.Instance.PlayerController;
            if (!playerController) return;

            playerController.SwitchSpell(spellIndex);

            if (!targetIcon) return;
            Spell[] spells = playerController.GetSpells();
            if (spellIndex < spells.Length && spells[spellIndex] != null) {
                GameManager.Instance.SpellIconManager?.Get(spells[spellIndex].GetIcon());
            }
        }
    }
}