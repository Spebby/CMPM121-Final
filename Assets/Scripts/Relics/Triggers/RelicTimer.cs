using System;
using System.Collections;
using CMPM.Core;
using CMPM.Relics.Effects;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;
using Random = UnityEngine.Random;


namespace CMPM.Relics.Triggers {
    public class RelicTimer : RelicCoroutineTrigger, IRPNEvaluator {
        protected RPNRange Range;
        public Action OnTriggered;
        bool _ran = false;
        
        public RelicTimer(IRelicEffect innerEffect, RPNRange range) : base(innerEffect) {
            Range = range;
        }

        // tbh I'm not sure what this should really signify. Revisit this when less tired
        public override bool Evaluate() => _ran;

        public override void OnTrigger(Action callback) {
            _ran = false;
            base.OnTrigger(callback);
        }

        public override void OnCancel() {
            _ran = false;
            base.OnCancel();
        }
        
        protected override IEnumerator RunCoroutine() {
            SerializedDictionary<string, float> vars = GetRPNVariables();
            yield return new WaitForSeconds(Random.Range(Range.Min.Evaluate(vars), Range.Max.Evaluate(vars)));
            _ran = true;
            
            InnerEffect.ApplyEffect();
            OnTriggered?.Invoke();
        }

        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentWave }
            };
        }
    }
}