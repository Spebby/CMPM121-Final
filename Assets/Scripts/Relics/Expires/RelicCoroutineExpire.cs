using System;
using System.Collections;
using CMPM.Core;
using CMPM.Relics.Effects;
using UnityEngine;


namespace CMPM.Relics.Expires {
    public abstract class RelicCoroutineExpire : RelicExpire {
        protected Coroutine Runner;

        protected RelicCoroutineExpire(IRelicEffect innerEffect) : base(innerEffect) { }
        ~RelicCoroutineExpire() => OnCancel();
        
        public override void OnTrigger(Action callback) {
            OnCancel();
            Runner = CoroutineManager.Instance.StartCoroutine(RunCoroutine());
            callback?.Invoke();
        }

        public virtual void OnCancel() {
            if (Runner == null) return;
            CoroutineManager.Instance.StopCoroutine(Runner);
            Runner = null;
            InnerEffect.RevertEffect();
        }

        protected abstract IEnumerator RunCoroutine();
    }
}