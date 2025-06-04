using CMPM.Level;
using TMPro;
using UnityEngine;


namespace CMPM.UI {
    public class MenuSelectorController : MonoBehaviour {
        public TextMeshProUGUI label;
        public string level;
        public EnemySpawner spawner;
        public AudioClip uiClickClip;
        public AudioSource uiAudioSource;

        GameObject _panel;

        void OnEnable() {
            _panel ??= GameObject.FindWithTag($"UIPanelPopup");
            if (!_panel) throw new MissingComponentException("UIPanelPopup not found");
        }

        public void SetLevel(string text) {
            level      = text;
            label.text = text;
        }

        public void StartLevel() {
            //button.onClick.RemoveAllListeners();
            //button.onClick.AddListener(() =>{
            if (uiAudioSource && uiClickClip)
            {
                uiAudioSource.PlayOneShot(uiClickClip);
            }
            _panel.SetActive(false);
            spawner.StartLevel(level);
        }
    }
}