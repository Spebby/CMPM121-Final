using CMPM.Core;
using CMPM.Movement;
using Pathfinding;
using UnityEngine;


namespace CMPM.AI.BehaviourTree.Actions {
    public class Flee : BehaviourTree {
        #region Readonlys
        readonly float _escapedDistance;
        #endregion

        public Flee(float escapedDistance) : base()
        {
            _escapedDistance = escapedDistance;
        }

        public override Result Run()
        {
            // AIPath path = Agent.GetComponent<AIPath>();
            // path.maxSpeed = Agent.Speed;
            // if (Vector2.Distance(GameManager.Instance.Player.transform.position, Agent.transform.position) < 1)
            // {
            //     path.destination = Agent.transform.position;
            // }
            // else
            // {
            //     path.destination = GameManager.Instance.Player.transform.position;
            // }


            Vector3 direction = Agent.transform.position - GameManager.Instance.Player.transform.position;
            if (direction.magnitude > _escapedDistance)
            {
                Agent.GetComponent<Unit>().movement = new Vector2(0, 0);
                return Result.SUCCESS;
            }

            //direction.x = direction.x * -1;
            //direction.y = direction.y * -1;
            Agent.GetComponent<Unit>().movement = direction.normalized * Agent.Speed;
            return Result.IN_PROGRESS;
        }

        public override BehaviourTree Copy() {
            return new Flee(_escapedDistance);
        }
    }
}