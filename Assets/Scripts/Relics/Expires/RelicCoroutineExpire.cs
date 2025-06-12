using System;
using System.Collections;
using CMPM.Core;
using UnityEngine;


namespace CMPM.Relics.Expires {
    public abstract class RelicCoroutineExpire : RelicExpire {
        protected Coroutine Runner;

        protected RelicCoroutineExpire(in Relic parent) : base(parent) { }
        
        public override void OnTrigger(Action callback) {
            OnCancel();
            Runner = CoroutineManager.Instance.StartCoroutine(RunCoroutine());
            callback?.Invoke();
        }

        public virtual void OnCancel() {
            if (Runner == null) return;
            CoroutineManager.Instance.StopCoroutine(Runner);
            Runner = null;
            Parent.OnDeactivate();
        }

        protected abstract IEnumerator RunCoroutine();
    }
}