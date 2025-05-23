using System;
using CMPM.Relics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    public class RelicSelector : MonoBehaviour {
        public RelicIcon icon;
        public Button button;
        public TMP_Text text;

        public void Set(RelicData relic, Action<RelicData> callback) {
            text.text = relic.Name;
            icon.Init(relic);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                callback(relic);
            });
        }
    }
}