using CMPM.Level;
using UnityEngine;

namespace CMPM
{
    public class DebugInputPolling : MonoBehaviour
    {
        public Item itemGameObject;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                itemGameObject._type = Item.ItemType.RELIC;
                var _new = Instantiate(itemGameObject);
                _new.transform.position = new Vector3(11, 5, -1);
            }
        }
    }
}
