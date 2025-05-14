using System.Collections.Generic;
using CMPM.Movement;


namespace CMPM.AI.BehaviorTree {
    public class BehaviorTree {
        #region Publics
        public enum Result {
            SUCCESS,
            FAILURE,
            IN_PROGRESS
        };

        public EnemyController Agent;
        #endregion

        public BehaviorTree() { }

        public virtual Result Run() {
            return Result.SUCCESS;
        }

        public void SetAgent(EnemyController agent) {
            Agent = agent;
        }

        public virtual IEnumerable<BehaviorTree> AllNodes() {
            yield return this;
        }

        public virtual BehaviorTree Copy() {
            return null;
        }
    }
}