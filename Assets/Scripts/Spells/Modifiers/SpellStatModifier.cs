using CMPM.Utils;


namespace CMPM.Spells.Modifiers {
    public class SpellStatModifier : SpellModifier {
        public SpellStatModifier(RPNString? damageModifier = null,
                                 RPNString? manaModifier = null,
                                 RPNString? speedModifier = null,
                                 RPNString? hitCapModifier = null,
                                 RPNString? cooldownModifier = null,
                                 RPNString? lifetimeModifier = null,
                                 RPNString? countModifier = null)
            : base(damageModifier, manaModifier, speedModifier, hitCapModifier, cooldownModifier, lifetimeModifier, countModifier) { }
    }
}