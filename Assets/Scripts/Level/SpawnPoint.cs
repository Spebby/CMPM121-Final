using UnityEngine;


namespace CMPM.Level {
    public class SpawnPoint : MonoBehaviour {
        public enum SpawnName {
            RED,
            GREEN,
            BONE
        }

        public SpawnName kind;
    }
}