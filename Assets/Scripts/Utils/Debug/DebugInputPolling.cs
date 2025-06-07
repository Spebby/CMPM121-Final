using CMPM.Level;
using UnityEngine;

namespace CMPM.Utils.Debug
{
    public class DebugInputPolling : MonoBehaviour
    {
        public LootController lc;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                lc._type = LootController.ItemType.RELIC;
                var _new = Instantiate(lc);
                _new.transform.position = new Vector3(Random.Range(-5,6), Random.Range(-5,6), -1);
            }
        }
    }
}
