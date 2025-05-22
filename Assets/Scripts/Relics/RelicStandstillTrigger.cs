using System;
using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Relics {
    public class RelicStandstillTrigger : RelicTrigger, IRPNEvaluator {
        protected RPNString WaitDuration;
        protected Coroutine Runner;
        
        public RelicStandstillTrigger(RelicEffect innerEffect, RPNString waitDuration) : base(innerEffect) {
            WaitDuration = waitDuration;
        }

        ~RelicStandstillTrigger() {
            CoroutineManager.Instance.StopCoroutine(Runner);
        }

        // Attempt to run the coroutine. If one is already running, then restart the counter.
        public override void OnTrigger(Action callback) {
            if (Runner != null) {
                CoroutineManager.Instance.StopCoroutine(Runner);
            }

            float time = WaitDuration.Evaluate(GetRPNVariables());
            Runner = CoroutineManager.Instance.StartCoroutine(WaitThenTrigger(time));
            callback?.Invoke();
        }

        public override bool Evaluate() {
            return !GameManager.Instance.PlayerController.IsMoving();
        }

        IEnumerator WaitThenTrigger(float time) {
            yield return new WaitForSeconds(time);
            if (!GameManager.Instance.PlayerController.IsMoving()) {
                InnerEffect.ApplyEffect();
            }

            Runner = null;
        }

        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentWave }
            };
        }
    }
}