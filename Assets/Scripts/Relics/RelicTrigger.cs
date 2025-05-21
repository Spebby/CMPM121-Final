namespace CMPM.Relics {
    public abstract class RelicTrigger {
        protected readonly RelicEffect InnerEffect;

        public RelicTrigger(RelicEffect innerEffect) {
            InnerEffect = innerEffect;
        }
        
        public virtual void OnTrigger() { InnerEffect.ApplyEffect(); }
        public virtual void OnCancel() { InnerEffect.RevertEffect(); }
        public virtual bool Evaluate() => true;
    }
}