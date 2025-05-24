using CMPM.Core;
using TMPro;
using UnityEngine;


namespace CMPM.UI {
    public class WaveLabelController : MonoBehaviour {
        TextMeshProUGUI _tmp;

        void Start() {
            _tmp = GetComponent<TextMeshProUGUI>();
        }

        void Update() {
            _tmp.text = GameManager.Instance.State switch {
                GameManager.GameState.INWAVE    => "Enemies left: " + GameManager.Instance.EnemiesLeft,
                GameManager.GameState.COUNTDOWN => "Starting in " + GameManager.Instance.Countdown,
                _                               => _tmp.text
            };
        }
    }
}