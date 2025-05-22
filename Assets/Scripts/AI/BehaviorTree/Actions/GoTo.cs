using CMPM.Movement;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Actions {
    public class GoTo : BehaviourTree {
        #region Readonlys
        readonly Transform _target;
        readonly float _arrivedDistance;
        #endregion

        public GoTo(Transform target, float arrivedDistance) : base() {
            _target          = target;
            _arrivedDistance = arrivedDistance;
        }

        public override Result Run() {
            Vector3 direction = _target.position - Agent.transform.position;
            if (direction.magnitude < _arrivedDistance) {
                Agent.GetComponent<Unit>().movement = new Vector2(0, 0);
                return Result.SUCCESS;
            }

            Agent.GetComponent<Unit>().movement = direction.normalized;
            return Result.IN_PROGRESS;
        }

        public override BehaviourTree Copy() {
            return new GoTo(_target, _arrivedDistance);
        }
    }
}