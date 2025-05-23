using CMPM.Core;
using CMPM.Relics;
using UnityEngine;


namespace CMPM.UI {
    public class RelicUIManager : MonoBehaviour {
        public GameObject relicUIPrefab;

        void OnEnable() {
            EventBus.Instance.OnRelicPickup += OnRelicPickup;
        }

        void OnDisable() {
            EventBus.Instance.OnRelicPickup -= OnRelicPickup;
        }

        public void OnRelicPickup(Relic r) {
            // make a new Relic UI representation
            GameObject rui = Instantiate(relicUIPrefab, transform);
            //rui.transform.localPosition = new Vector3(-450 + 40 * (player.Relics.Count - 1), 0, 0);
            RelicUI ruic = rui.GetComponent<RelicUI>();
            ruic.Init(r);
        }
    }
}