using CMPM.Core;
using CMPM.Enemies;


namespace CMPM.AI.BehaviorTree.Actions {
    public class Attack : BehaviorTree {
        public Attack() : base() { }

        public override Result Run() {
            EnemyAction act = Agent.GetAction(EnemyActionTypes.Attack);
            if (act == null) return Result.FAILURE;

            bool success = act.Do(GameManager.Instance.Player.transform);
            return success ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviorTree Copy() {
            return new Attack();
        }
    }
}