using System;
using CMPM.Relics.Effects;


namespace CMPM.Relics.Expires {
    public abstract class RelicExpire {
        protected readonly Relic Parent;

        public RelicExpire(in Relic parent) {
            Parent = parent;
        }
        
        public virtual void OnTrigger(Action callback) {
            Parent.OnDeactivate();
            callback?.Invoke();
        }
    }
}