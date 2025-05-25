using System;
using CMPM.Relics.Effects;


namespace CMPM.Relics.Triggers {
    public abstract class RelicTrigger {
        protected readonly Relic Parent;

        protected RelicTrigger(in Relic parent) {
            Parent = parent;
        }

        public virtual void OnTrigger(Action callback) {
            Parent.OnActivate();
            callback?.Invoke();
        }
        public virtual void OnCancel() { Parent.OnDeactivate(); }
        public virtual bool Evaluate() => true;
    }
}