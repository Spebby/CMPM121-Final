using UnityEngine;


namespace CMPM.AI.Util {
    public class AIWaypoint : MonoBehaviour {
        #region Publics
        public enum Type {
            SAFE,
            FORWARD,
            WALL
        }

        public Vector3 position;
        public Type type;
        #endregion

        void Start() {
            position = transform.position;
            AIWaypointManager.Instance.AddWaypoint(this);
        }
    }
}