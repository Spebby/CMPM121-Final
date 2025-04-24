using System.Collections.Generic;
using UnityEngine;

// Serializing Dictionary
namespace CMPM.Structures {
    [System.Serializable]
    public class Hashtable<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] List<TKey> keys = new();
        [SerializeField] List<TValue> values = new();

        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this) {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize() {
            Clear();
            if (keys.Count != values.Count) {
                throw new System.Exception("keys count does not match values count");
            }

            for (int i = 0; i < keys.Count; i++) {
                Add(keys[i], values[i]);
            }
        }
    }
}