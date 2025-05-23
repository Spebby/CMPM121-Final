using System;
using CMPM.Relics;
using UnityEngine;


namespace CMPM.UI {
    public class RelicSelectorManager : MonoBehaviour {
        RelicSelector[] _items;

        void Start() {
            _items = GetComponentsInChildren<RelicSelector>();
        }

        public void Set(RelicData[] relics, Action<RelicData> callback) {
            _items ??= GetComponentsInChildren<RelicSelector>();
            
            int count = Mathf.Min(_items.Length, relics.Length);
            for (int i = 0; i < count; i++) {
                RelicSelector item = _items[i];
                item.Set(relics[i], callback);
                item.gameObject.SetActive(true);
            }
            
            // Disable remaining items.
            for (int i = count; i < _items.Length; i++) {
                _items[i].gameObject.SetActive(false);
            }
        }
    }
}