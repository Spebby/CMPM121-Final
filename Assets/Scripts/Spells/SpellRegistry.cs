using CMPM.Spells.Modifiers;
using CMPM.Utils.Structures;


// ReSharper disable ClassNeverInstantiated.Global
namespace CMPM.Spells {
    public class SpellRegistry : Registry<int, SpellData> { }

    public class SpellModifierRegistry : Registry<int, ISpellModifier> { }

    public class SpellModifierDataRegistry : Registry<int, SpellModifierData> { }
}
