using System.Collections.Generic;
using CMPM.Core;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Queries {
    public class NearbyEnemiesQueryReversed : BehaviourTree {
        #region Readonlys
        readonly int _count;
        readonly float _distance;
        #endregion

        public NearbyEnemiesQueryReversed(int count, float distance) : base() {
            _count    = count;
            _distance = distance;
        }

        public override Result Run() {
            List<GameObject> nearby          = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, _distance);
            bool             success         = true;
            bool             hasSupport      = false;
            EnemyController  enemyController = Agent.GetComponent<EnemyController>();
            
            if      (enemyController.type == BehaviourType.Support) hasSupport = true;
            if      (nearby.Count < _count) success = false;
            
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (GameObject enemy in nearby) {
                if (enemy.GetComponent<EnemyController>().type == BehaviourType.Support) hasSupport = true;
            }

            if (hasSupport) success = false;
            return success ? Result.FAILURE : Result.SUCCESS;
        }

        public override BehaviourTree Copy() {
            return new NearbyEnemiesQueryReversed(_count, _distance);
        }
    }
}