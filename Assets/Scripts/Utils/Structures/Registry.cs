using System;
using System.Collections.Generic;
using System.Linq;


namespace CMPM.Utils.Structures {
    public abstract class Registry<S, T> {
        protected static readonly Dictionary<S, T> REGISTRY = new();
        protected static readonly Random RNG = new();

        public static void Register(S key, T val) {
            REGISTRY[key] = val;
        }

        public static T Get(S key) {
            return REGISTRY.GetValueOrDefault(key);
        }

        public static T GetRandom() {
            return REGISTRY[GetRandomKey()];
        }

        public static S GetRandomKey() {
            return REGISTRY.Keys.ElementAt(RNG.Next(REGISTRY.Count));
        }

        public static void SetRegistry(Dictionary<S, T> newRegistry) {
            REGISTRY.Clear();
            foreach (KeyValuePair<S, T> kvp in newRegistry) {
                REGISTRY.Add(kvp.Key, kvp.Value);
            }
        }

        public static Dictionary<S, T>.KeyCollection GetHashes() {
            return REGISTRY.Keys;
        }

        public static int Count => REGISTRY.Count;
    }
}