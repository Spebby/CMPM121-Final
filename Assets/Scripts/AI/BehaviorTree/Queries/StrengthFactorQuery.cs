using CMPM.Core;
using CMPM.Enemies;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviorTree.Queries {
    public class StrengthFactorQuery : BehaviorTree {
        #region Readonlys
        readonly float _minStrengthFactor;
        #endregion

        public StrengthFactorQuery(float minStrengthFactor) : base() {
            _minStrengthFactor = minStrengthFactor;
        }

        public override Result Run() {
            GameObject target = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
            return ((EnemyAttack)target.GetComponent<EnemyController>().GetAction("attack")).StrengthFactor
                >= _minStrengthFactor
                ? Result.SUCCESS
                : Result.FAILURE;
        }

        public override BehaviorTree Copy() {
            return new StrengthFactorQuery(_minStrengthFactor);
        }
    }
}