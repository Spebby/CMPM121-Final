using System;
using System.Collections.Generic;
using System.Linq;


namespace CMPM {
    public abstract class Registry<T> {
        protected static readonly Dictionary<int, T> REGISTRY = new();
        
        public static void Register(int hash, T val) => REGISTRY[hash] = val;
        public static T Get(int hash) => REGISTRY.GetValueOrDefault(hash);
        public static T GetRandom()   => REGISTRY[GetRandomHash()];
        public static int GetRandomHash () => REGISTRY.Keys.ElementAt(new Random().Next(REGISTRY.Count));
        
        public static Dictionary<int,T>.KeyCollection GetHashes() => REGISTRY.Keys;
    }
}