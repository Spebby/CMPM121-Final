using CMPM.Core;
using CMPM.Enemies;
using UnityEngine;


namespace CMPM.AI.BehaviorTree.Actions {
    public class Buff : BehaviorTree {
        public Buff() : base() { }

        public override Result Run() {
            GameObject  target = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
            EnemyAction act    = Agent.GetAction("buff");
            if (act == null) return Result.FAILURE;

            bool success = act.Do(target.transform);
            return success ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviorTree Copy() {
            return new Buff();
        }
    }
}