using NUnit.Framework.Constraints;


namespace CMPM.Relics {
    public abstract class RelicPrecondition {
        public virtual bool Evaluate() => true;
    }
}