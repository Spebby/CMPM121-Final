using System;
using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;
using Random = UnityEngine.Random;


namespace CMPM.Relics {
    public class RelicTimerExpire : RelicExpire, IRPNEvaluator {
        protected RPNRange Range;
        protected Coroutine Runner;
        
        public RelicTimerExpire(RelicEffect innerEffect, RPNRange range) : base(innerEffect) {
            Range = range;
        }

        ~RelicTimerExpire() {
            if (Runner == null) return;
            CoroutineManager.Instance.StopCoroutine(Runner);
            InnerEffect.RevertEffect();
        }

        // Attempt to run the coroutine. If one is already running, then restart the counter.
        public override void OnTrigger(Action callback) {
            if (Runner != null) {
                CoroutineManager.Instance.StopCoroutine(Runner);
            }
            
            Runner = CoroutineManager.Instance.StartCoroutine(WaitThenTrigger());
            callback?.Invoke();
        }

        public void OnCancel() {
            CoroutineManager.Instance.StopCoroutine(Runner);
            Runner = null;
        }
        
        IEnumerator WaitThenTrigger() {
            while (true) {
                SerializedDictionary<string, float> table = GetRPNVariables();
                yield return new WaitForSeconds(Random.Range(Range.Min.Evaluate(table), Range.Max.Evaluate(table)));
                InnerEffect.RevertEffect();
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