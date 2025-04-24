using System;
using UnityEngine;


namespace CMPM.Level {
    public class SpawnPoint : MonoBehaviour {
        // WARNING: Additional entries *MUST* update [[../Utils/SpawnLocationParser.cs]]
        public enum SpawnName {
            RED,
            GREEN,
            BONE,
            RANDOM
        }

        public SpawnName kind;

        void OnEnable() {
            if (kind == SpawnName.RANDOM) {
                throw new NotSupportedException(
                    $"Spawn point {gameObject.name} 'kind' is set to type 'RANDOM', which is not supported.");
            }
        }
    }
}