using System;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;


namespace CMPM.Relics.Triggers {
    public class RelicKillCondition : RelicTrigger, IRPNEvaluator {
        protected readonly RPNString Amount;
        int _internalCounter;

        public RelicKillCondition(in Relic parent, RPNString amount) : base(in parent) {
            Amount           = amount;
            _internalCounter = 0;
        }

        public override void OnTrigger(Action callback = null) {
            _internalCounter++;
            
            if (_internalCounter < Amount.Evaluate(GetRPNVariables())) return;
            _internalCounter = 0;
            base.OnTrigger(callback);
            callback?.Invoke();
        }
        
        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentWave }
            };
        }
    }
}