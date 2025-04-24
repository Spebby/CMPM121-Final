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
            _tmp.text = GameManager.INSTANCE.State switch {
                GameManager.GameState.INWAVE    => "Enemies left: " + GameManager.INSTANCE.EnemyCount,
                GameManager.GameState.COUNTDOWN => "Starting in " + GameManager.INSTANCE.Countdown,
                _                               => _tmp.text
            };
        }
    }
}