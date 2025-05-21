using System;
using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;
using Random = UnityEngine.Random;


namespace CMPM.Relics {
    public class RelicTimerTrigger : RelicTrigger, IRPNEvaluator {
        protected RPNRange Range;
        protected Coroutine Runner;
        protected readonly bool Overwrite;

        public Action OnTriggered;
        
        public RelicTimerTrigger(RelicEffect innerEffect, RPNRange range, bool overwrite) : base(innerEffect) {
            Range = range;
            Overwrite = overwrite;
        }

        ~RelicTimerTrigger() {
            CoroutineManager.Instance.StopCoroutine(Runner);
        }

        // Attempt to run the coroutine. If one is already running, then restart the counter.
        public override void OnTrigger() {
            if (Runner != null) {
                CoroutineManager.Instance.StopCoroutine(Runner);
            }
            
            Runner = CoroutineManager.Instance.StartCoroutine(WaitThenTrigger());
        }

        public override void OnCancel() {
            CoroutineManager.Instance.StopCoroutine(Runner);
            Runner = null;
        }

        public override bool Evaluate() => true;

        IEnumerator WaitThenTrigger() {
            while (true) {
                SerializedDictionary<string, float> table = GetRPNVariables();
                yield return new WaitForSeconds(Random.Range(Range.Min.Evaluate(table), Range.Max.Evaluate(table)));
                if (Overwrite) InnerEffect.RevertEffect();
                InnerEffect.ApplyEffect();
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