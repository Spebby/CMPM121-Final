using System;
using System.Runtime.CompilerServices;
using CMPM.Utils;
using UnityEngine;
using UnityEngine.Serialization;


[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
namespace CMPM.Level {
    [CreateAssetMenu(fileName = "LootTable", menuName = "Level/LootTable")]
    public class LootTable : ScriptableObject {
        public LootController.LootType[] types;
    }
}