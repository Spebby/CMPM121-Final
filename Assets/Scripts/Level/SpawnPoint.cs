using System;
using CMPM.Utils.LevelParsing;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.Level {
    public class SpawnPoint : MonoBehaviour {
        // WARNING: Additional entries *MUST* update [[../Utils/SpawnLocationParser.cs]]
        [JsonConverter(typeof(SpawnLocationParser))]
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