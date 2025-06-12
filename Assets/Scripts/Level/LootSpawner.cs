using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;


namespace CMPM.Level {
    public class LootSpawner : MonoBehaviour{
        [SerializeField] LootController prefab;

        public GameObject Spawn([CanBeNull] LootTable lootTable) {
            LootController.LootType type = LootController.LootType.RELIC;
            if (lootTable) type = lootTable.types[Random.Range(0, lootTable.types.Length)];
            prefab.type = type;
            return Instantiate(prefab, transform).gameObject;
        }
    }
}