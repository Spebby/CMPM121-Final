using TMPro;
using UnityEngine;


public class WaveLabelController : MonoBehaviour {
    TextMeshProUGUI _tmp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _tmp = this.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.State == GameManager.GameState.INWAVE) {
            _tmp.text = "Enemies left: " + GameManager.Instance.EnemyCount;
        }

        if (GameManager.Instance.State == GameManager.GameState.COUNTDOWN) {
            _tmp.text = "Starting in " + GameManager.Instance.Countdown;
        }
    }
}