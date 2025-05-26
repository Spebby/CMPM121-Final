using System;
using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;
using Random = UnityEngine.Random;


namespace CMPM.Relics.Triggers {
    public class RelicTimer : RelicCoroutineTrigger, IRPNEvaluator {
        protected RPNRange Range;
        public Action OnTriggered;
        bool _ran;
        
        public RelicTimer(in Relic parent, in RPNRange range) : base(parent) {
            Range = range;
        }

        // tbh I'm not sure what this should really signify. Revisit this when less tired
        public override bool Evaluate() => _ran;

        public override void OnTrigger(Action callback = null) {
            _ran = false;
            base.OnTrigger(callback);
        }

        public override void OnCancel() {
            _ran = false;
            base.OnCancel();
        }
       
        // TODO: review if this is actually what we want. It's a bit messy but should be fine?
        protected override IEnumerator RunCoroutine() {
            while (true) {
                SerializedDictionary<string, float> vars = GetRPNVariables();
                yield return new WaitForSeconds(Random.Range(Range.Min.Evaluate(vars), Range.Max.Evaluate(vars)));
                _ran = true;

                Parent.OnActivate();
                OnTriggered?.Invoke();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentWave }
            };
        }
    }
}