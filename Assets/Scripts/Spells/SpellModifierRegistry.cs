using System.Collections.Generic;


namespace CMPM.Spells {
    public static class SpellModifierRegistry {
        static readonly Dictionary<uint, ISpellModifier> MODIFIER = new();

        public static void Register(uint hash, ISpellModifier mod) => MODIFIER[hash] = mod;
        public static ISpellModifier Get(uint hash) => MODIFIER.GetValueOrDefault(hash);
    }
}