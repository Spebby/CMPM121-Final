using CMPM.Core;
using CMPM.Spells;
using CMPM.Utils;
using CMPM.Utils.Structures;


namespace CMPM.Relics {
	public class GainManaEffect : RelicEffect, IRPNEvaluator {
		protected readonly RPNString Amount;

		public GainManaEffect(SpellCaster caster, RPNString amount) : base(caster) {
			Amount = amount;
		}

		public override void ApplyEffect() {
			Caster.ModifyMana((int)Amount.Evaluate(GetRPNVariables()));
		}

		public override void RevertEffect() {
			Caster.ModifyMana(-(int)Amount.Evaluate(GetRPNVariables()));
		}

		public override bool CanCancel() => false;

		public SerializedDictionary<string, float> GetRPNVariables() {
			return new SerializedDictionary<string, float> {
				{ "wave", GameManager.Instance.CurrentWave }
			};
		}
	}
}
