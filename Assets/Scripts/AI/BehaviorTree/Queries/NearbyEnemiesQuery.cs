using System.Collections.Generic;
using CMPM.Core;
using UnityEngine;


namespace CMPM.AI.BehaviorTree.Queries {
    public class NearbyEnemiesQuery : BehaviorTree {
        #region Readonlys
        readonly int _count;
        readonly float _distance;
        #endregion

        public NearbyEnemiesQuery(int count, float distance) : base() {
            _count    = count;
            _distance = distance;
        }

        public override Result Run() {
            List<GameObject> nearby = GameManager.Instance.GetEnemiesInRange(Agent.transform.position, _distance);
            return nearby.Count >= _count ? Result.SUCCESS : Result.FAILURE;
        }

        public override BehaviorTree Copy() {
            return new NearbyEnemiesQuery(_count, _distance);
        }
    }
}