using System.Collections.Generic;
using CMPM.Core;
using CMPM.Spells;
using CMPM.Utils;
using CMPM.Utils.Structures;


namespace CMPM.Relics {
	public class GainSpellpowerEffect : RelicEffect, IRPNEvaluator {
		protected readonly RPNString Amount;

		readonly Stack<int> _stack = new();
		
		public GainSpellpowerEffect(SpellCaster caster, RPNString amount) : base(caster) {
			Amount = amount;
		}

		public override void ApplyEffect() {
			int i = (int)Amount.Evaluate(GetRPNVariables());
			Caster.ModifySpellpower(i);
			_stack.Push(i);
		}

		public override void RevertEffect() {
			if (!CanCancel()) return;
			Caster.ModifyMana(-_stack.Pop());
		}

		public override bool CanCancel() => _stack.Count > 0;

		public SerializedDictionary<string, float> GetRPNVariables() {
			return new SerializedDictionary<string, float> {
				{ "wave", GameManager.Instance.CurrentWave }
			};
		}
	}
}
