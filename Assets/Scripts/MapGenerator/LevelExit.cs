using CMPM.Core;
using UnityEngine;
using static CMPM.MapGenerator.Room;


namespace CMPM.MapGenerator {
    public class LevelExit : MonoBehaviour {
        void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;

            const int ANCHOR = GRID_SIZE / 2;
            GameManager.Instance.Player.transform.position = new Vector3(ANCHOR, ANCHOR, 0);
            EventBus.Instance.DoFloorClear();
            
            GameManager.Instance.SetState(GameManager.GameState.FLOOREND);
        }
    }
}
