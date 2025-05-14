namespace CMPM.AI.BehaviorTree.Queries {
    public class AbilityReadyQuery : BehaviorTree {
        #region Readonlys
        readonly string _ability;
        #endregion

        public AbilityReadyQuery(string ability) : base() {
            _ability = ability;
        }

        public override Result Run() {
            return Agent.GetAction(_ability).Ready() ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviorTree Copy() {
            return new AbilityReadyQuery(_ability);
        }
    }
}