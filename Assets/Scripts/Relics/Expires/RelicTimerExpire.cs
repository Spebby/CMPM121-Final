using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;
using Random = UnityEngine.Random;


namespace CMPM.Relics.Expires {
    public class RelicTimerExpire : RelicCoroutineExpire, IRPNEvaluator {
        protected RPNRange Range;
        
        public RelicTimerExpire(in Relic parent, RPNRange range) : base(parent) {
            Range = range;
        }

        protected override IEnumerator RunCoroutine() {
            SerializedDictionary<string, float> table = GetRPNVariables();
            yield return new WaitForSeconds(Random.Range(Range.Min.Evaluate(table), Range.Max.Evaluate(table)));
            Parent.OnDeactivate();
        }

        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentWave }
            };
        }
    }
}