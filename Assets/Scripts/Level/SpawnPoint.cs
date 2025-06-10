using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.Level {
    public class SpawnPoint : MonoBehaviour {
        // WARNING: Additional entries *MUST* update [[../Utils/SpawnLocationParser.cs]]
        public enum SpawnName {
            RED,
            GREEN,
            BONE,
            RANDOM
        }

        [FormerlySerializedAs("kind")] public SpawnName Kind;

        void OnEnable() {
            if (Kind == SpawnName.RANDOM) {
                throw new NotSupportedException($"Spawn point {gameObject.name} 'kind' is set to type 'RANDOM', which is not supported.");
            }
        }
    }
}