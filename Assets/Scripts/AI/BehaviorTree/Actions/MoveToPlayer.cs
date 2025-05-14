using System.Collections.Generic;
using CMPM.Core;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviorTree.Actions {
    public class MoveToPlayer : BehaviorTree {
        #region Readonlys
        readonly float _arrivedDistance;
        #endregion

        public MoveToPlayer(float arrivedDistance) : base() {
            _arrivedDistance = arrivedDistance;
        }

        public override Result Run() {
            Vector3 direction = GameManager.Instance.Player.transform.position - Agent.transform.position;
            if (direction.magnitude < _arrivedDistance) {
                Agent.GetComponent<Unit>().movement = new Vector2(0, 0);
                return Result.SUCCESS;
            }

            int ticks = 0;
            if (Agent.GetComponent<EnemyController>().monster == "skeleton"
             || Agent.GetComponent<EnemyController>().monster == "warlock") {
                List<GameObject> nearby = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, 8f);
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (GameObject enemy in nearby) {
                    if (enemy.GetComponent<EnemyController>().monster != "zombie") continue;
                    if (!(direction.magnitude
                        > (GameManager.Instance.Player.transform.position - enemy.transform.position).magnitude)
                     || !((GameManager.Instance.Player.transform.position - enemy.transform.position).magnitude
                        > 3f)) continue;
                    direction = Agent.transform.position - GameManager.Instance.Player.transform.position;
                    ticks++;
                    Debug.Log("I AM A RETARD");
                }
            }

            Agent.GetComponent<Unit>().movement = ticks == 0
                ? direction.normalized
                : (Agent.transform.position - GameManager.Instance.Player.transform.position).normalized;


            return Result.IN_PROGRESS;
        }

        public override BehaviorTree Copy() {
            return new MoveToPlayer(_arrivedDistance);
        }
    }
}