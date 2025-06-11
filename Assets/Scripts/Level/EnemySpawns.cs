using System;
using System.Runtime.CompilerServices;
using CMPM.Utils;
using UnityEngine;
using UnityEngine.Serialization;


[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
namespace CMPM.Level {
    [CreateAssetMenu(fileName = "EnemySpawns", menuName = "Level/EnemySpawns")]
    public class RoomSpawn : ScriptableObject {
        public Spawn[] spawns;
    }
    
    [Serializable]
    public struct Spawn {
        //[JsonConverter(typeof(SpawnEnemyParser))] <-- this is only valid syntax when it's a parameterless constructor
        public string EnemyName;
        public int MinFloor;
        public RPNString HPFormula;
        public RPNString SpeedFormula;
        public RPNString DamageFormula;

        public int Weight;

        [FormerlySerializedAs("Location")] public SpawnPoint.SpawnName[] Locations;
    }
}