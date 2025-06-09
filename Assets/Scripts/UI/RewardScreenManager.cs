using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Relics;
using static CMPM.Core.GameManager.GameState;
using CMPM.Spells;
using CMPM.Utils;
using Newtonsoft.Json;
using UnityEngine.Events;
using UnityEngine.Serialization;


// This entire class will become redundant during the pivot :(
namespace CMPM.UI {
    public class RewardScreenManager : MonoBehaviour {
        [Header("Panels & Buttons")]
        [SerializeField] GameObject panel;

        [SerializeField] GameObject endUI;
        [SerializeField] Button nextButton;
        [SerializeField] Button acceptButton;

        [Header("Main Menu")]
        [SerializeField] GameObject classSelector;

        [SerializeField] GameObject classProfilePrefab;
        [SerializeField] GameObject difficultySelector;
        
        [Header("Spell UI")]
        [SerializeField] GameObject spellUI;
        [SerializeField] SpellUI spellUIIcon;
        [SerializeField] TextMeshProUGUI spellText;
        [SerializeField] int maxSpellModifiers = 3;

        [Header("Relic UI")]
        [SerializeField] GameObject relicUI;
        [SerializeField] RelicSelectorManager relicSelectorManager;
        [SerializeField] GameObject relicSelectorPrefab;
        [SerializeField] int relicRewardFrequency = 3;
        [SerializeField] int relicRewardAmount = 3;
        
        [Header("Discard UI")]
        [SerializeField] GameObject discardSpellUI;

        [SerializeField] Transform discardSpellUIContainer;
        [FormerlySerializedAs("discardSpellButton")] [SerializeField] Button discardSpellBackButton;

        [Header("Stats")]
        [SerializeField] TextMeshProUGUI statsText;

        PlayerController _player;
        Spell _rewardSpell;

        //for button click sounds to play
        [Header("Sounds")]
        [SerializeField] public AudioClip uiClickClip;
        [SerializeField] public AudioSource uiAudioSource;

        [SerializeField] public AudioClip uiRewardScreenClip;
        [SerializeField] public AudioSource uiRewardAudioSource;
        [SerializeField] public AudioClip gameOverScreenClip;
        [SerializeField] public AudioSource gameOverScreenSource;


        [FormerlySerializedAs("OnPanelClose")]
        [Header("Unity Events")] // I don't like Unity Events that much, but they are convenient from time to time.
        [SerializeField] UnityEvent onPanelClose;

        // Temporary stat collectors
        double _timeSpent;
        int _damageDone;
        int _damageTaken;

        enum RelicClaimed {
            None,
            Generated,
            Claimed
        }
        RelicClaimed _relicClaimed;
        bool _spellClaimed;

        void Awake() {
            // Cache player reference
            // Unfortunately can't just ask GameManager for it since GameManager might not have it yet if we get unlucky w/ load order
            _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                
            // This doesn't really belong here, but I can't think of a better spot to put it...
            TextAsset json = Resources.Load<TextAsset>("classes");
            Dictionary<PlayerController.PlayerClass.Type, PlayerController.PlayerClass> classMap =
                JsonConvert
                   .DeserializeObject<Dictionary<PlayerController.PlayerClass.Type, PlayerController.PlayerClass>>(
                        json.text, new PlayerClassMapParser());
            foreach (KeyValuePair<PlayerController.PlayerClass.Type, PlayerController.PlayerClass> kvp in classMap) {
                ClassRegistry.Register(kvp.Key, kvp.Value);
            }

            for (int i = 0; i < relicRewardAmount; i++) {
                Instantiate(relicSelectorPrefab, relicUI.transform);
            }
        }

        void Start() {
            Dictionary<PlayerController.PlayerClass.Type, PlayerController.PlayerClass>.KeyCollection classes = ClassRegistry.GetHashes();
            foreach (PlayerController.PlayerClass.Type key in classes) {
                PlayerController.PlayerClass c        = ClassRegistry.Get(key);
                GameObject                   _        = Instantiate(classProfilePrefab, classSelector.transform);
                ClassSelector                selector = _.GetComponent<ClassSelector>();
                Button                       button   = _.GetComponent<Button>();
                selector.Init(c);
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>{
                if (uiAudioSource && uiClickClip)
                    {
                        uiAudioSource.PlayOneShot(uiClickClip);
                }
                    GameManager.Instance.PlayerController.UpdateClass(c);
                    classSelector.SetActive(false);
                    difficultySelector.SetActive(true);
                });
            }
            
            nextButton.onClick.AddListener(ClosePanel);
            discardSpellBackButton.onClick.AddListener(ShowRewardScreen);
            HandleGameStateChanged(PREGAME);
        }

        void OnEnable() {
            GameManager.OnStateChanged += HandleGameStateChanged;
            EventBus.Instance.OnDamage += UpdateDamageStats;
        }

        void OnDisable() {
            GameManager.OnStateChanged -= HandleGameStateChanged;
            EventBus.Instance.OnDamage -= UpdateDamageStats;
        }

        public void ResetGame() {
            GameManager.Instance.SetState(PREGAME);
            _player.ClearSpells();
        }

        void HandleGameStateChanged(GameManager.GameState newState) {
            acceptButton.onClick.RemoveAllListeners();
            _relicClaimed = RelicClaimed.None;
            _spellClaimed = false;
            
            switch (newState) {
                case PREGAME:
                    ShowStartScreen();
                    break;

                case INWAVE:
                    // we continue tracking time in Update()
                    break;

                case WAVEEND:
                    ShowRewardScreen();
                    break;

                case GAMEOVER:
                    ShowLossScreen();
                    break;

                case COUNTDOWN:
                default:
                    HideAllUI();
                    break;
            }
        }

        void Update() {
            if (GameManager.Instance.State != INWAVE) return;
            _timeSpent += Time.deltaTime;
        }

        #region Screens
        void ShowStartScreen() {
            panel.SetActive(true);
            
            classSelector.SetActive(true);
            difficultySelector.SetActive(false);
            
            endUI.SetActive(false);
            nextButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            spellUI.SetActive(false);
            relicUI.SetActive(false);
            discardSpellUI.SetActive(false);
            statsText.text = string.Empty;
        }

        void ShowLossScreen() {
            if (gameOverScreenSource && gameOverScreenClip)
            {
                gameOverScreenSource.PlayOneShot(gameOverScreenClip);
            }
            panel.SetActive(true);
            classSelector.SetActive(false);
            difficultySelector.SetActive(false);
            endUI.SetActive(true);
            nextButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            spellUI.SetActive(false);
            discardSpellUI.SetActive(false);

            statsText.text = $"Time Spent: {_timeSpent:F2}\t" +
                             $"Damage Done: {_damageDone}\t" +
                             $"Damage Taken: {_damageTaken}";
        }

        void ShowRewardScreen() {
            if (uiRewardAudioSource && uiRewardScreenClip)
            {
                uiRewardAudioSource.PlayOneShot(uiRewardScreenClip);
            }
            panel.SetActive(true);
            classSelector.SetActive(false);
            difficultySelector.SetActive(false);
            endUI.SetActive(true);
            nextButton.gameObject.SetActive(true);
            acceptButton.gameObject.SetActive(true);
            spellUI.SetActive(true);
            relicUI.SetActive(false);
            discardSpellUI.SetActive(false);

            // Pick and display one random spell
            if (_rewardSpell == null)
            {
                _rewardSpell = SpellBuilder.MakeRandomSpell(_player, maxSpellModifiers);
                spellUIIcon.SetSpell(_rewardSpell);
                spellText.text = $"{_rewardSpell.GetName()}\n{_rewardSpell.GetDescription()}";
            }
            else
            {
                spellUI.SetActive(false);
                acceptButton.gameObject.SetActive(false);
            }

            UpdateStatsText();

            // Wire up the accept button
            acceptButton.onClick.AddListener(OnAcceptClicked);
            if (_relicClaimed == RelicClaimed.Generated) {
                relicUI.SetActive(true);
            }
            
            // Wave isn't updated until the Enemy Spawner starts spawning.
            int wave = GameManager.Instance.CurrentWave;
            if (_relicClaimed != RelicClaimed.None || wave % relicRewardFrequency != 0) return;
            relicUI.SetActive(true);
            _relicClaimed = RelicClaimed.Generated;
            
            BitArray set = _player.RelicOwnership;
            RelicData[] relics = RelicBuilder.GetRelicSet(set, relicRewardAmount, out BitArray _);
            if (relics.Length == 0) return;
            relicSelectorManager.Set(relics, r => {
                _relicClaimed = RelicClaimed.Claimed;
                relicUI.SetActive(false);
                _player.AddRelic(RelicBuilder.CreateRelic(r));
                set.Set(RelicRegistry.GetIndexFromRelic(r), true);
                if (_spellClaimed) {
                    ClosePanel();
                }
            });
        }

        void ShowDiscardMenu() {
            endUI.SetActive(false);
            discardSpellUI.SetActive(true);

            Spell[]   spells = _player.GetSpells();
            SpellUI[] slots  = discardSpellUIContainer.GetComponentsInChildren<SpellUI>();

            for (int i = 0; i < slots.Length; i++) {
                SpellUI slot = slots[i];
                slot.SetSpell(spells[i]);
                if (i < spells.Length) {
                    slot.SetSpell(spells[i]);

                    Button btn = slot.GetComponentInChildren<Button>();
                    btn.onClick.RemoveAllListeners();
                    int idx = i;
                    btn.onClick.AddListener(() => {
                        if (uiAudioSource && uiClickClip)
                        {
                            uiAudioSource.PlayOneShot(uiClickClip);
                        }
                        _player.AddSpell(_rewardSpell, idx);
                        if (_relicClaimed == RelicClaimed.Claimed) {
                            ClosePanel();
                        } else {
                            ShowRewardScreen();
                        }
                    });
                } else {
                    slot.gameObject.SetActive(false);
                }
            }
        }

        void HideAllUI() {
            panel.SetActive(false);
        }
        #endregion

        void OnAcceptClicked() {
            if (uiAudioSource && uiClickClip)
            {
                uiAudioSource.PlayOneShot(uiClickClip);
            }
            _spellClaimed = true;
            if (_player.HasSpellRoom()) {
                _player.AddSpell(_rewardSpell);
                spellUI.gameObject.SetActive(false);
                acceptButton.gameObject.SetActive(false);
                if (_relicClaimed == RelicClaimed.Claimed) {
                    ClosePanel();
                }
            } else {
                ShowDiscardMenu();
            }
        }

        void ClosePanel() {
            if (uiAudioSource && uiClickClip)
            {
                uiAudioSource.PlayOneShot(uiClickClip);
            }
            panel.SetActive(false);
            nextButton.gameObject.SetActive(false);
            acceptButton.onClick.RemoveAllListeners();
            _rewardSpell = null;
            onPanelClose?.Invoke();
        }

        void UpdateStatsText() {
            int    cur      = GameManager.Instance.CurrentWave;
            int    tot      = GameManager.Instance.TotalWaves;
            string waveInfo = $"\tWave: {cur}" + (tot > 0 ? $"/{tot}" : "");

            if (tot > 0 && cur == tot)
                nextButton.gameObject.SetActive(false);

            statsText.text =
                $"Time Spent: {_timeSpent:F2}\t" +
                $"Damage Done: {_damageDone}\t" +
                $"Damage Taken: {_damageTaken}{waveInfo}";
        }

        void UpdateDamageStats(Vector3 _, Damage damage, Hittable target) {
            if (target.Owner == GameManager.Instance.Player) {
                _damageTaken += damage.Amount;
            } else {
                _damageDone += damage.Amount;
            }
        }
        
        #region Debug
        public void DebugAddSpell() {
            _rewardSpell ??= SpellBuilder.MakeRandomSpell(_player, maxSpellModifiers);
            _player.AddSpell(_rewardSpell, 3);
            _rewardSpell = null;
        }
        #endregion
    }
}