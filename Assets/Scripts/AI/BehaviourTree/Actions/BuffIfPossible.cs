using System.Collections.Generic;
using CMPM.Core;
using CMPM.Enemies;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Actions {
    public class BuffIfPossible : BehaviourTree {
        public BuffIfPossible() : base() { }

        public override Result Run() {
            GameObject  target = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
            EnemyAction act    = Agent.GetAction(EnemyActionTypes.Buff);
            if (act == null) return Result.FAILURE;
            bool success = false;

            if (!Agent.GetAction(EnemyActionTypes.Buff).Ready()) return Result.FAILURE;
            List<GameObject> nearby = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, Agent.GetAction(EnemyActionTypes.Buff).Range);

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (GameObject enemy in nearby) {
                EnemyController reference = enemy.GetComponent<EnemyController>();
                if (!reference.canBeBuffed) continue;
                success = act.Do(target.transform);
                break;
            }

            return success ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviourTree Copy() {
            return new BuffIfPossible();
        }
    }
}