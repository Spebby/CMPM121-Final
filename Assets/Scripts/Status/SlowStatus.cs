using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Status {
    /* This may cause problems later, but for the moment I'm doing delta here incase we wanted to stack
     * this effect. My idea being that if we just stored original speed, a bad ordering could cause the
     * original speed to be lost. So instead we just "record" the amount we've slowed them by.
     * But due to floater bullshit there'll inevitably be some degradation.
     */
    public class SlowStatus : AbstractStatus {
        readonly RPNString _slowFactor;
        float _delta;

        public SlowStatus(Entity target, RPNString duration, RPNString factor) : base(target, duration) {
            _slowFactor = factor;
        }

        public override void RemoveStatus() {
            if (Target) {
                Target.ModifySpeed(_delta);
            }

            base.RemoveStatus();
        }
        
        protected override IEnumerator RunCoroutine() {
            if (!Target) yield break;
            _delta            =  Target.unit.speed * _slowFactor.Evaluate(GetRPNVariables());
            Target.ModifySpeed(-_delta);
            yield return new WaitForSeconds(Duration.Evaluate(GetRPNVariables()));
            RemoveStatus();
        }
    }
}