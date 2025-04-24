using CMPM.Core;
using UnityEngine;


namespace CMPM.UI {
    public class RewardScreenManager : MonoBehaviour {
        public GameObject rewardUI;
        
        // Update is called once per frame
        void LateUpdate() {
            rewardUI.SetActive(GameManager.INSTANCE.State == GameManager.GameState.WAVEEND);
        }
    }
}