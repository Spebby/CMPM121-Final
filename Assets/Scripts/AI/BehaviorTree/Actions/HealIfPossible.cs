using System.Collections.Generic;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Enemies;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Actions {
    public class HealIfPossible : BehaviourTree {
        public HealIfPossible() : base() { }

        public override Result Run() {
            GameObject  target = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
            EnemyAction act    = Agent.GetAction(EnemyActionTypes.Heal);
            if (act == null) return Result.FAILURE;
            bool success = false;

            if (!Agent.GetAction(EnemyActionTypes.Heal).Ready()) return Result.FAILURE;
            List<GameObject> nearby =
                GameManager.Instance.GetEnemiesInRange(Agent.transform.position, Agent.GetAction(EnemyActionTypes.Heal).Range);

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (GameObject enemy in nearby) {
                Hittable healthInfo = enemy.GetComponent<EnemyController>().HP;
                if (healthInfo.MinHP >= healthInfo.MaxHP) continue;
                success = act.Do(target.transform);
                break;
            }

            return success ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviourTree Copy() {
            return new HealIfPossible();
        }
    }
}