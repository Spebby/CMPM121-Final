using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Relics.Triggers {
    public class RelicStandstill : RelicCoroutineTrigger, IRPNEvaluator {
        protected RPNString WaitDuration;
        
        public RelicStandstill(in Relic parent, in RPNString waitDuration) : base(parent) {
            WaitDuration = waitDuration;
        }

        public override bool Evaluate() => !GameManager.Instance.PlayerController.IsMoving();

        
        protected override IEnumerator RunCoroutine() {
            float time = WaitDuration.Evaluate(GetRPNVariables());
            yield return new WaitForSeconds(time);

            if (!GameManager.Instance.PlayerController.IsMoving()) {
                Parent.OnActivate();
            }

            Runner = null;
        }

        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentFloor }
            };
        }
    }
}