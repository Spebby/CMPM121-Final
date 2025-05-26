using System.Collections.Generic;
using System.Linq;


namespace CMPM.AI.BehaviourTree {
    public class InteriorNode : BehaviourTree {
        #region Protected
        protected readonly List<BehaviourTree> Children;
        protected int CurrentChild;
        #endregion

        public InteriorNode(IEnumerable<BehaviourTree> children) : base() {
            Children = new List<BehaviourTree>();
            Children.AddRange(children);
            CurrentChild = 0;
        }

        public List<BehaviourTree> CopyChildren() {
            return Children.Select(c => c.Copy()).ToList();
        }

        public override IEnumerable<BehaviourTree> AllNodes() {
            yield return this;
            foreach (BehaviourTree n in Children.SelectMany(c => c.AllNodes())) {
                yield return n;
            }
        }
    }
}