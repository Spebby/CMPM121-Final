using CMPM.Level;
using UnityEngine;

namespace CMPM.Utils.Debug {
    public class DebugInputPolling : MonoBehaviour {
        [SerializeField] LootController.LootType type = LootController.LootType.RELIC;
        [SerializeField] LootController lc;
        
        void Update() {
            if (!Input.GetKeyDown(KeyCode.Backspace)) return;
            lc.type = type;
            // lc._type = LootController.LootType.RELIC;
            LootController _new = Instantiate(lc);
            _new.transform.position = new Vector3(Random.Range(2,8), Random.Range(2,8), -1);
        }
    }
}
