using System;
using CMPM.Relics;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.UI {
    public class RelicSelector : MonoBehaviour {
        GameObject[] _items;

        void Start() {
            _items = GetComponentsInChildren<GameObject>();
            HideAll();
        }

        public void HideAll() {
            foreach (GameObject item in _items) {
                item.SetActive(false);
            }
        }

        public void Set(Relic[] relics, Action<Relic> callback) {
            int count = Mathf.Min(_items.Length, relics.Length);
            for (int i = 0; i < count; i++) {
                Relic relic = relics[i];
                GameObject item = _items[i];
                
                item.SetActive(true);
                RelicUI rui = item.GetComponentInChildren<RelicUI>();
                rui.Init(relic);
                
                Button button = item.GetComponentInChildren<Button>();
                button.onClick.RemoveAllListeners();

                button.onClick.AddListener(() => {
                    callback(relic);
                });
            }
            
            // Disable remaining items.
            for (int i = count; i < _items.Length; i++) {
                _items[i].SetActive(false);
            }
        }
    }
}