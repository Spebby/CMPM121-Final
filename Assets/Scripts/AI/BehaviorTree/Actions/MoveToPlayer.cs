using System.Collections.Generic;
using CMPM.Core;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Actions {
    public class MoveToPlayer : BehaviourTree {
        #region Readonlys
        readonly float _arrivedDistance;
        #endregion

        public MoveToPlayer(float arrivedDistance) : base() {
            _arrivedDistance = arrivedDistance;
        }

        public override Result Run() {
            Vector3 playerPos = GameManager.Instance.Player.transform.position;
            Vector3 direction = playerPos - Agent.transform.position;
            if (direction.magnitude < _arrivedDistance) {
                Agent.GetComponent<Unit>().movement = new Vector2(0, 0);
                return Result.SUCCESS;
            }

            int ticks = 0;
            // Review this. The idea is that enemies flee if blocked by certain enemies. I need to work this logic
            // into the data more concretely. I could potentially assign behaviour tags to enemies and enemies that
            // "can be blocked" check if an blocking enemy is infront of them ?
            
            // Original logic was skeletons and warlocks. For now just supports can be blocked.
            if (Agent.GetComponent<EnemyController>().type == BehaviourType.Support) {
                List<GameObject> nearby = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, 8f);
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (GameObject enemy in nearby) {
                    // if (enemy.GetComponent<EnemyController>().monster != "zombie") continue; // 
                    float player2EnemyDistance = (playerPos - enemy.transform.position).magnitude;
                    if (direction.magnitude > player2EnemyDistance && player2EnemyDistance > 3f) continue;
                    direction = Agent.transform.position - playerPos;
                    ticks++;
                }
            }

            Agent.GetComponent<Unit>().movement = ticks == 0
                ? direction.normalized
                : (Agent.transform.position - playerPos).normalized;


            return Result.IN_PROGRESS;
        }

        public override BehaviourTree Copy() {
            return new MoveToPlayer(_arrivedDistance);
        }
    }
}