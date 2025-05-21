using System;
using System.Collections.Generic;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using Random = UnityEngine.Random;


namespace CMPM.Relics {
	public class GainRandomBuff : RelicEffect, IRPNEvaluator {
		protected readonly RPNString MaxFactor;
		protected readonly PlayerController Player;

		#region Buff Management
		enum BuffType {
			Spellpower,
			MaxMana,
			ManaRegenRate,
			Speed
		}
		readonly struct BuffRule {
			public readonly Func<float> GetBase;
			public readonly Action<int> Modify;

			public BuffRule(Func<float> getBase, Action<int> modify) {
				GetBase = getBase;
				Modify  = modify;
			}
		}
		static readonly BuffType[] BUFFS = { BuffType.Spellpower, BuffType.MaxMana, BuffType.ManaRegenRate, BuffType.Speed };
		readonly Dictionary<BuffType, BuffRule> _buffRules;
		readonly Stack<Tuple<Action<int>, int>> _stack = new();
		#endregion	

		// It would be nice to generalise this to "any entity that can cast a spell"
		public GainRandomBuff(PlayerController player, RPNString factor) : base(player) {
			MaxFactor = factor;
			Player = player;

			_buffRules = new Dictionary<BuffType, BuffRule> {
				{ BuffType.Spellpower,    new BuffRule(() => Caster.SpellPower, Caster.ModifySpellpower) },
				{ BuffType.MaxMana,       new BuffRule(() => Caster.MaxMana,    Caster.ModifyMaxMana) },
				{ BuffType.ManaRegenRate, new BuffRule(() => Caster.ManaRegen,  Caster.ModifyManaRegen) },
				{ BuffType.Speed,         new BuffRule(() => Player.Speed,      Player.ModifySpeed) }
			};
		}

		// If I were a braver man I would use GCHandle and skip all this delegate nonsense
		public override void ApplyEffect() {
			BuffType buff   = BUFFS[Random.Range(0, BUFFS.Length)];
			float    factor = MaxFactor.Evaluate(GetRPNVariables());
			BuffRule rule   = _buffRules[buff];

			int increment = (int)(rule.GetBase() * factor);

			_stack.Push(new Tuple<Action<int>, int>(rule.Modify, increment));
			rule.Modify(increment);
		}

		public override void RevertEffect() {
			if (!CanCancel()) return;
			
			Tuple<Action<int>, int> x = _stack.Pop();
			x.Item1(-x.Item2);
		}

		public override bool CanCancel() => _stack.Count > 0;

		public SerializedDictionary<string, float> GetRPNVariables() {
			return new SerializedDictionary<string, float> {
				{ "wave", GameManager.Instance.CurrentWave }
			};
		}
	}
}
