using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Spells;
using TMPro;
using UnityEngine;
using static CMPM.Core.GameManager.GameState;


namespace CMPM.UI {
    public class RewardScreenManager : MonoBehaviour {
        public GameObject endUI;
        public GameObject nextButton;

        public GameObject regUI;
        public TextMeshProUGUI stats;
        GameObject _panel;


        [Header("Spell UI")] public GameObject acceptButton;
        public GameObject spellUI;
        public SpellUI spellUIIcon;
        public TextMeshProUGUI spellText;
        public GameObject discardSpellUI;

        Spell _rewardSpell;

        // I don't have time to give it a proper spot for now, so stat collection is going in here for the moment
        double _timeSpent;
        int _damageDone;
        int _damageTaken;
        
        PlayerController _player;
        
        void OnEnable() {
            _panel ??= GameObject.FindWithTag($"UIPanelPopup");
            if (!_panel) throw new MissingComponentException("UIPanelPopup not found");
            EventBus.Instance.OnDamage += UpdateDamageStats;
        }

        void OnDisable() {
            EventBus.Instance.OnDamage -= UpdateDamageStats;
        }

        // Update is called once per frame
        void LateUpdate() {
            // this is ass
            switch (GameManager.Instance.State) {
                case WAVEEND:
                    SetRewardScreen();
                    break;
                case GAMEOVER:
                    SetLossScreen();
                    break;
                case INWAVE:
                    _timeSpent += Time.deltaTime;
                    break;
                case PREGAME:
                    SetStartScreen();
                    break;
                case COUNTDOWN:
                default:
                    DisableUI();
                    break;
            }
        }

        public void SetStartScreen() {
            GameManager.Instance.State = PREGAME;
            _panel.SetActive(true);
            regUI.SetActive(true);
            endUI.SetActive(false);
        }

        void UpdateDamageStats(Vector3 target, Damage damage, Hittable hittable) {
            if (hittable.Owner != GameManager.Instance.Player) {
                _damageDone += damage.Amount;
                return;
            }
            // ^ This assumes that only the player can damage other entities.

            _damageTaken += damage.Amount;
        }

        void SetLossScreen() {
            _panel.SetActive(true);
            regUI.SetActive(false);
            endUI.SetActive(true);
            nextButton.SetActive(false);
            acceptButton.SetActive(false);
            spellUI.SetActive(false);

            stats.text = $"Time Spent: {_timeSpent:F2}\tDamage Done: {_damageDone}\tDamage Taken: {_damageTaken}";
        }

        void SetRewardScreen() {
            regUI.SetActive(false);
            _panel.SetActive(true);
            endUI.SetActive(true);
            nextButton.SetActive(true);
            acceptButton.SetActive(true);
            spellUI.SetActive(true);

            if (!_player) _player = GameManager.Instance.Player.GetComponent<PlayerController>();
            if (_rewardSpell == null) {
                _rewardSpell =
                    SpellBuilder.MakeRandomSpell(_player);
                spellUIIcon.SetSpell(_rewardSpell);
                spellText.text = $"{_rewardSpell.GetName()}\n{_rewardSpell.GetDescription()}";
            }


            string extra    = "";
            string waveText = $"\tWave: {GameManager.Instance.currentWave}";
            if (GameManager.Instance.totalWaves > 0) {
                waveText += $"/{GameManager.Instance.totalWaves}";
                if (GameManager.Instance.currentWave == GameManager.Instance.totalWaves) {
                    extra = "\nYou Win!";
                    nextButton.SetActive(false);
                }
            }

            stats.text =
                $"Time Spent: {_timeSpent:F2}\tDamage Done: {_damageDone}\tDamage Taken: {_damageTaken}{waveText}{extra}";
        }

        public void AcceptSpell() {
            if (!_player) _player = GameManager.Instance.Player.GetComponent<PlayerController>();
            if (_player.HasSpellRoom()) {
                _player.AddNewSpell(_rewardSpell);
            } else {
                spellUI.gameObject.SetActive(false);
                discardSpellUI.SetActive(true);
            }
        }

        public void DisableUI() { 
            _panel.SetActive(false);
            _rewardSpell = null;
        }
    }
}