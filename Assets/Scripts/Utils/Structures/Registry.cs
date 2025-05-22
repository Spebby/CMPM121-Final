using System;
using System.Collections.Generic;
using System.Linq;


namespace CMPM.Utils.Structures {
    public abstract class Registry<T> {
        protected static readonly Dictionary<int, T> REGISTRY = new();
        protected static readonly Random RNG = new();

        public static void Register(int hash, T val) {
            REGISTRY[hash] = val;
        }

        public static T Get(int hash) {
            return REGISTRY.GetValueOrDefault(hash);
        }

        public static T GetRandom() {
            return REGISTRY[GetRandomHash()];
        }

        public static int GetRandomHash() {
            return REGISTRY.Keys.ElementAt(RNG.Next(REGISTRY.Count));
        }

        public static Dictionary<int, T>.KeyCollection GetHashes() {
            return REGISTRY.Keys;
        }

        public static int Count => REGISTRY.Count;
    }
}