using CMPM.Spells;


namespace CMPM.Relics {
	public abstract class RelicEffect {
		protected readonly SpellCaster Caster;

		public RelicEffect(SpellCaster caster) {
			Caster = caster;
		}

		public abstract void ApplyEffect();
		public abstract void RevertEffect();

		public abstract bool CanCancel();
	}
}
