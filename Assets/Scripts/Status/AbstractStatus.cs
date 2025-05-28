using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using JetBrains.Annotations;
using UnityEngine;


namespace CMPM.Status {
    public abstract class AbstractStatus : IStatusEffect, IRPNEvaluator {
        protected readonly RPNString Duration;
        [CanBeNull] protected readonly Entity Target;
        protected Coroutine Runner;

        protected AbstractStatus(Entity target, RPNString duration) {
            Target = target;
            Duration = duration;
        }

        ~AbstractStatus() {
            RemoveStatus();
        }

        public virtual void ApplyStatus() {
            RemoveStatus();
            Runner = CoroutineManager.Instance.StartCoroutine(RunCoroutine());
        }

        public virtual void RemoveStatus() {
            OnCancel();
        }

        protected abstract IEnumerator RunCoroutine();
        protected virtual void OnCancel() {
            if (Runner == null) return;
            CoroutineManager.Instance.StopCoroutine(Runner);
            Runner = null;
        }
        
        public SerializedDictionary<string, float> GetRPNVariables() {
            return new SerializedDictionary<string, float> {
                { "wave", GameManager.Instance.CurrentWave }
            };
        } 
    }
    
    public static class StatusEffects {
        public static (int, float, int) CalculateTicks(float duration, int damage, float weight) {
            int   n         = Mathf.Max(1, Mathf.Min(damage, Mathf.RoundToInt(damage / weight)));
            int   dpt       = damage / n;
            float t         = duration / n;
            return (n, t, dpt);
        }
    }

    public enum StatusEffect {
        Icy,
        Burn
    }
}