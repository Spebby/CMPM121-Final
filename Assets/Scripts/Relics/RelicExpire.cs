namespace CMPM.Relics {
    public abstract class RelicExpire {
        protected readonly RelicEffect InnerEffect;

        public RelicExpire(RelicEffect innerEffect) {
            InnerEffect = innerEffect;
        }
        
        public virtual void OnTrigger() { InnerEffect.RevertEffect(); }
    }
}