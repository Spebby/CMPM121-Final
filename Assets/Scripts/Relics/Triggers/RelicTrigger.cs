using System;


namespace CMPM.Relics.Triggers {
    public abstract class RelicTrigger {
        protected readonly Relic Parent;

        protected RelicTrigger(in Relic parent) {
            Parent = parent;
        }

        public virtual void OnTrigger(Action callback = null) {
            Parent.OnActivate();
            callback?.Invoke();
        }
        public virtual void OnCancel() { Parent.OnDeactivate(); }
        public virtual bool Evaluate() => true;
    }
}