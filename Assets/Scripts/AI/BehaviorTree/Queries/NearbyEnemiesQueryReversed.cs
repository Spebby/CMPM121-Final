using System.Collections.Generic;
using CMPM.Core;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviorTree.Queries {
    public class NearbyEnemiesQueryReversed : BehaviorTree {
        #region Readonlys
        readonly int _count;
        readonly float _distance;
        #endregion

        public NearbyEnemiesQueryReversed(int count, float distance) : base() {
            _count    = count;
            _distance = distance;
        }

        public override Result Run() {
            List<GameObject> nearby      = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, _distance);
            bool             success     = true;
            bool             hasWarlock  = false;
            bool             hasSkeleton = false;
            if (Agent.GetComponent<EnemyController>().monster == "skeleton")
                hasSkeleton                                                                 = true;
            else if (Agent.GetComponent<EnemyController>().monster == "warlock") hasWarlock = true;
            if (nearby.Count < _count) success = false;
            foreach (GameObject enemy in nearby) {
                if (enemy.GetComponent<EnemyController>().monster == "warlock")
                    hasWarlock                                                                    = true;
                else if (enemy.GetComponent<EnemyController>().monster == "skeleton") hasSkeleton = true;
            }

            if (!hasWarlock && !hasSkeleton) success = false;
            return success ? Result.FAILURE : Result.SUCCESS;
        }

        public override BehaviorTree Copy() {
            return new NearbyEnemiesQueryReversed(_count, _distance);
        }
    }
}