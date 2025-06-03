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
                // left off here need to determine the ItemType before
                // instantiation
                itemGameObject._type = Item.ItemType.SPELL;
                var _new = Instantiate(itemGameObject);
                _new.transform.position = new Vector3(11, 5, -1);
            }
        }
    }
}
