using CMPM.Core;
using CMPM.Enemies;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Actions {
    public class Buff : BehaviourTree {
        public Buff() : base() { }

        public override Result Run() {
            GameObject  target = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
            EnemyAction act    = Agent.GetAction(EnemyActionTypes.Buff);
            if (act == null) return Result.FAILURE;

            bool success = act.Do(target.transform);
            return success ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviourTree Copy() {
            return new Buff();
        }
    }
}