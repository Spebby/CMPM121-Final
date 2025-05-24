using System;
using System.Collections.Generic;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;


namespace CMPM.Relics.Effects {
    // Generic Stat Modifier effect.
    public class GainStatEffect : IRelicEffect, IRPNEvaluator {
        protected readonly RPNString Amount;
        protected readonly Action<int> ModifierCallback;
        readonly Stack<int> _stack = new();

        public GainStatEffect(RPNString amount, Action<int> modifierCallback) {
            Amount           = amount;
            ModifierCallback = modifierCallback;
        }

        public void ApplyEffect() {
            int amount = (int)Amount.Evaluate(GetRPNVariables());
            ModifierCallback(amount);
            _stack.Push(amount);
        }

        public void RevertEffect() {
            if (!CanCancel()) return;
            ModifierCallback(-_stack.Pop());
        }

        public bool CanCancel() => _stack.Count > 0;

        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentWave }
            };
        }
    }
}