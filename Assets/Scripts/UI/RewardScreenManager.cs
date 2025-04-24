using CMPM.Core;
using UnityEngine;


namespace CMPM.UI {
    public class RewardScreenManager : MonoBehaviour {
        public GameObject rewardUI;
        GameObject _panel;
            
        void OnEnable() {
            _panel ??= GameObject.FindWithTag($"UIPanelPopup");
            if (!_panel) throw new MissingComponentException("UIPanelPopup not found");
        }
        
        // Update is called once per frame
        void LateUpdate() {
            if (GameManager.INSTANCE.State != GameManager.GameState.WAVEEND) return;
            _panel.SetActive(true);
            rewardUI.SetActive(true);
        }
    }
}