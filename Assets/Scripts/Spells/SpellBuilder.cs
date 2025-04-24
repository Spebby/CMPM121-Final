namespace CMPM.Spells {
    public class SpellBuilder {
        public Spell Build(SpellCaster owner) {
            return new Spell(owner);
        }
    }
}