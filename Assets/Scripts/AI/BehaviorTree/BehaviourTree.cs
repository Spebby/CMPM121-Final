using System.Collections.Generic;
using CMPM.Movement;


namespace CMPM.AI.BehaviourTree {
    public class BehaviourTree {
        #region Publics
        public enum Result {
            SUCCESS,
            FAILURE,
            IN_PROGRESS
        };

        public EnemyController Agent;
        #endregion

        public virtual Result Run() {
            return Result.SUCCESS;
        }

        public void SetAgent(EnemyController agent) {
            Agent = agent;
        }

        public virtual IEnumerable<BehaviourTree> AllNodes() {
            yield return this;
        }

        public virtual BehaviourTree Copy() {
            return null;
        }
    }
}