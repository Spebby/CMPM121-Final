using System;
using CMPM.Relics.Effects;


namespace CMPM.Relics.Expires {
    public abstract class RelicExpire {
        protected readonly IRelicEffect InnerEffect;

        public RelicExpire(IRelicEffect innerEffect) {
            InnerEffect = innerEffect;
        }
        
        public virtual void OnTrigger(Action callback) {
            InnerEffect.RevertEffect();
            callback?.Invoke();
        }
    }
}