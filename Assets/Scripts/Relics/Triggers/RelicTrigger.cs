using System;
using CMPM.Relics.Effects;


namespace CMPM.Relics.Triggers {
    public abstract class RelicTrigger {
        protected readonly IRelicEffect InnerEffect;

        protected RelicTrigger(IRelicEffect innerEffect) {
            InnerEffect = innerEffect;
        }

        public virtual void OnTrigger(Action callback) {
            InnerEffect.ApplyEffect();
            callback?.Invoke();
        }
        public virtual void OnCancel() { InnerEffect.RevertEffect(); }
        public virtual bool Evaluate() => true;
    }
}