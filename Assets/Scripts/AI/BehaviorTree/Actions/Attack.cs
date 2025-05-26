using CMPM.Core;
using CMPM.Enemies;


namespace CMPM.AI.BehaviourTree.Actions {
    public class Attack : BehaviourTree {
        public Attack() : base() { }

        public override Result Run() {
            EnemyAction act = Agent.GetAction(EnemyActionTypes.Attack);
            if (act == null) return Result.FAILURE;

            bool success = act.Do(GameManager.Instance.Player.transform);
            return success ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviourTree Copy() {
            return new Attack();
        }
    }
}