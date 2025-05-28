using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Status {
    // ReSharper disable once InconsistentNaming
    public class DOTStatus : AbstractStatus {
        protected readonly Damage.Type Type;
        protected readonly RPNString Amount;

        public DOTStatus(Entity target, RPNString duration, RPNString amount, Damage.Type type) : base(target, duration) {
            Amount = amount;
            Type   = type;
        }
        
        protected override IEnumerator RunCoroutine() {
            SerializedDictionary<string, float> table = GetRPNVariables();
            int amount = (int)Amount.Evaluate(table);
            (int n, float t, int dpt) = StatusEffects.CalculateTicks(Duration.Evaluate(GetRPNVariables()), amount, 0.2f);
            Damage dmg = new(dpt, Type);
            
            for (int i = 0; i < n; i++) {
                Target?.HP.Damage(dmg);
                if (!Target) yield break;
                yield return new WaitForSeconds(t);
            }

            int rem = amount % n; // n >= 1
            if (rem != 0) {
                Target?.HP.Damage(new Damage(dpt, Type));
            }
        }
    }
}