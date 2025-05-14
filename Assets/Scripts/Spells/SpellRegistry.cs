using CMPM.Spells.Modifiers;
using CMPM.Utils.Structures;


// ReSharper disable ClassNeverInstantiated.Global

namespace CMPM.Spells {
    public class SpellRegistry : Registry<SpellData> { }

    public class SpellModifierRegistry : Registry<ISpellModifier> { }

    public class SpellModifierDataRegistry : Registry<SpellModifierData> { }
}