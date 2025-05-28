using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Status {
    // ReSharper disable once InconsistentNaming
    public class DOTStatus : AbstractStatus {
        protected readonly RPNString Amount;
        protected readonly float Weight;
        protected readonly Damage.Type Type;

        public DOTStatus(Entity target, RPNString duration, RPNString amount, float weight, Damage.Type type) : base(target, duration) {
            Amount = amount;
            Weight = weight;
            Type   = type;
        }
        
        protected override IEnumerator RunCoroutine() {
            SerializedDictionary<string, float> table = GetRPNVariables();
            int amount = (int)Amount.Evaluate(table);
            (int n, float t, int dpt) = StatusEffects.CalculateTicks(Duration.Evaluate(GetRPNVariables()), amount, Weight);
            Damage dmg = new(dpt, Type);
            
            for (int i = 0; i < n; i++) {
                if (!Target) yield break;
                Target.HP.Damage(dmg);
                yield return new WaitForSeconds(t);
            }

            int rem = amount % n; // n >= 1
            if (Target && rem != 0) {
                Target.HP.Damage(new Damage(dpt, Type));
            }
        }
    }
}