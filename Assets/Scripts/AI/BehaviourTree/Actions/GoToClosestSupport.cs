using System.Collections.Generic;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Enemies;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Actions {
    public class GoToClosestSupport : BehaviourTree {
        #region Readonlys
        readonly Transform _target;
        readonly float _arrivedDistance;
        #endregion

        public GoToClosestSupport(float arrivedDistance) {
            _arrivedDistance = arrivedDistance;
        }

        public override Result Run() {
            Vector3 playerDirection = GameManager.Instance.Player.transform.position - Agent.transform.position;
            if (playerDirection.magnitude < 25) return Result.FAILURE;
            GameObject       closestEnemy = GameObject.Find("wp0"); // <-- This is bad
            List<GameObject> suds         = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, 8f);
            if (suds.Count >= 5) return Result.SUCCESS;

            if (!closestEnemy) return Result.IN_PROGRESS;
            Vector3 target    = closestEnemy.transform.position;
            Vector3 direction = target - Agent.transform.position;
            if (direction.magnitude - _arrivedDistance < -0.3f) {
                closestEnemy = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
                if (!closestEnemy) return Result.IN_PROGRESS;
                target    = closestEnemy.transform.position;
                direction = target - Agent.transform.position;
                
                ApplyAction(Agent.GetAction(EnemyActionTypes.Permabuff));
                ApplyAction(Agent.GetAction(EnemyActionTypes.Heal));

                EnemyController oEnemy     = closestEnemy.GetComponent<EnemyController>();
                BehaviourType    type       = Agent.type;
                int             order      = Agent.HP.HP  + (type == BehaviourType.Support ? 10000 : 0);
                int             otherOrder = oEnemy.HP.HP + (type == BehaviourType.Support ? 10000 : 0);
                
                //playerDirection is the distance from the player to the enemy, now we need to check the distance between the target and the player
                Vector3 playerToTarget = GameManager.Instance.Player.transform.position - target;
                // Repositioning
                if (otherOrder < order) {
                    Vector3 newPos = playerToTarget / 2;
                    newPos = direction - newPos;
                    newPos.Normalize();
                    Agent.GetComponent<Unit>().movement = newPos * Agent.Speed;
                    return Result.IN_PROGRESS;
                }

                if (otherOrder > order) {
                    Vector3 newPos = playerToTarget / 2;
                    newPos = direction + newPos;
                    newPos.Normalize();
                    Agent.GetComponent<Unit>().movement = newPos * Agent.Speed;
                    return Result.IN_PROGRESS;
                }

                Agent.GetComponent<Unit>().movement = new Vector2(0, 0);
                return Result.SUCCESS;
            }

            Agent.GetComponent<Unit>().movement = direction.normalized * Agent.Speed;
            return Result.IN_PROGRESS;
        }

        void ApplyAction(EnemyAction action) {
            if (!action.Ready()) return;
            
            List<GameObject> nearby = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, action.Range);
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (GameObject enemy in nearby) {
                Hittable healthInfo = enemy.GetComponent<EnemyController>().HP;
                if (healthInfo.MinHP >= healthInfo.MaxHP) continue;
                action.Do(enemy.transform);
                break;
            }
        }

        public override BehaviourTree Copy() {
            return new GoToClosestSupport(_arrivedDistance);
        }
    }
}