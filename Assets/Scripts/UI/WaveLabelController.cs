using CMPM.Core;
using TMPro;
using UnityEngine;


namespace CMPM.UI {
    public class WaveLabelController : MonoBehaviour {
        TextMeshProUGUI _tmp;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            _tmp = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update() {
            if (GameManager.INSTANCE.State == GameManager.GameState.INWAVE) {
                _tmp.text = "Enemies left: " + GameManager.INSTANCE.EnemyCount;
            }

            if (GameManager.INSTANCE.State == GameManager.GameState.COUNTDOWN) {
                _tmp.text = "Starting in " + GameManager.INSTANCE.Countdown;
            }
        }
    }
}