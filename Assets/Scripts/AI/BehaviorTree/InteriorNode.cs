using System.Collections.Generic;
using System.Linq;


namespace CMPM.AI.BehaviorTree {
    public class InteriorNode : BehaviorTree {
        #region Protected
        protected readonly List<BehaviorTree> Children;
        protected int CurrentChild;
        #endregion

        public InteriorNode(IEnumerable<BehaviorTree> children) : base() {
            Children = new List<BehaviorTree>();
            Children.AddRange(children);
            CurrentChild = 0;
        }

        public List<BehaviorTree> CopyChildren() {
            return Children.Select(c => c.Copy()).ToList();
        }

        public override IEnumerable<BehaviorTree> AllNodes() {
            yield return this;
            foreach (BehaviorTree n in Children.SelectMany(c => c.AllNodes())) {
                yield return n;
            }
        }
    }
}