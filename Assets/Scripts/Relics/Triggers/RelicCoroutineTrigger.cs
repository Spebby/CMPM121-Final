using System;
using System.Collections;
using CMPM.Core;
using UnityEngine;


namespace CMPM.Relics.Triggers {
    public abstract class RelicCoroutineTrigger : RelicTrigger {
        protected Coroutine Runner;

        protected RelicCoroutineTrigger(in Relic parent) : base(parent) { }
        
        public override void OnTrigger(Action callback = null) {
            OnCancel();
            Runner = CoroutineManager.Instance.StartCoroutine(RunCoroutine());
            callback?.Invoke();
        }

        public override void OnCancel() {
            if (Runner == null) return;
            CoroutineManager.Instance.StopCoroutine(Runner);
            Runner = null;
        }

        protected abstract IEnumerator RunCoroutine();
    }
}