using System;
using System.Collections;
using CMPM.Core;
using CMPM.Relics.Effects;
using UnityEngine;


namespace CMPM.Relics.Triggers {
    public abstract class RelicCoroutineTrigger : RelicTrigger {
        protected Coroutine Runner;

        protected RelicCoroutineTrigger(IRelicEffect innerEffect) : base(innerEffect) { }
        ~RelicCoroutineTrigger() {
            OnCancel();
        }
        
        public override void OnTrigger(Action callback) {
            OnCancel();
            Runner = CoroutineManager.Instance.StartCoroutine(RunCoroutine());
            callback?.Invoke();
        }

        public override void OnCancel() {
            if (Runner == null) return;
            CoroutineManager.Instance.StopCoroutine(Runner);
            Runner = null;
            InnerEffect.RevertEffect();
        }

        protected abstract IEnumerator RunCoroutine();
    }
}