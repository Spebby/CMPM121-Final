using CMPM.Core;
using CMPM.Enemies;
using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Queries {
    public class StrengthFactorQuery : BehaviourTree {
        #region Readonlys
        readonly float _minStrengthFactor;
        #endregion

        public StrengthFactorQuery(float minStrengthFactor) : base() {
            _minStrengthFactor = minStrengthFactor;
        }

        public override Result Run() {
            GameObject target = GameManager.Instance.GetClosestOtherEnemy(Agent.gameObject);
            return ((EnemyAttack)target.GetComponent<EnemyController>().GetAction(EnemyActionTypes.Attack)).StrengthFactor
                >= _minStrengthFactor
                ? Result.SUCCESS
                : Result.FAILURE;
        }

        public override BehaviourTree Copy() {
            return new StrengthFactorQuery(_minStrengthFactor);
        }
    }
}