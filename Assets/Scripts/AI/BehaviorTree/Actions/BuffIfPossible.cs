using System.Collections.Generic;
using CMPM.Core;
using CMPM.Enemies;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviorTree.Actions {
    public class BuffIfPossible : BehaviorTree {
        public BuffIfPossible() : base() { }

        public override Result Run() {
            GameObject  target = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
            EnemyAction act    = Agent.GetAction("buff");
            if (act == null) return Result.FAILURE;
            bool success = false;

            if (!Agent.GetAction("buff").Ready()) return Result.FAILURE;
            List<GameObject> nearby =
                GameManager.Instance.GetEnemiesInRange(Agent.transform.position, Agent.GetAction("buff").Range);

            foreach (GameObject enemy in nearby) {
                EnemyController reference = enemy.GetComponent<EnemyController>();
                if (reference.monster != "skeleton") continue;
                success = act.Do(target.transform);
                break;
            }

            return success ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviorTree Copy() {
            return new BuffIfPossible();
        }
    }
}