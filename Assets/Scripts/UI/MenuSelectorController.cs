using CMPM.Level;
using TMPro;
using UnityEngine;


namespace CMPM.UI {
    public class MenuSelectorController : MonoBehaviour {
        public TextMeshProUGUI label;
        public string level;
        public EnemySpawner spawner;

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
            _panel.SetActive(false); 
            spawner.StartLevel(level);
        }
    }
}