using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;


namespace CMPM.Relics {
    public class RelicRegistry {
        // replace this w/ a set
        protected static readonly List<RelicData> REGISTRY = new();
        protected static readonly Random RNG = new();
        
        public static void Register(RelicData val) {
            if (REGISTRY.Contains(val)) return;
            REGISTRY.Add(val);
        }

        public static RelicData GetRandom() {
            return REGISTRY[RNG.Next(REGISTRY.Count)];
        }

        public static int GetRandomUnique(in BitArray flags, out RelicData? relic) {
            List<int> unsetIndices = new();
            for (int i = 0; i < flags.Length; i++) {
                if (!flags[i]) unsetIndices.Add(i);
            }
            
            relic = null;
            if (unsetIndices.Count == 0) return -1;
            int index = RNG.Next(0, unsetIndices.Count);
            relic = REGISTRY[index];
            return index;
        }

        public static int GetIndexFromRelic(in Relic relic) {
            return REGISTRY.IndexOf(relic);
        }
        
        public static int Count => REGISTRY.Count; 
    }
}