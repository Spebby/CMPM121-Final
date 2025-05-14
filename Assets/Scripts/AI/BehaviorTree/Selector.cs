using System;
using System.Collections.Generic;


namespace CMPM.AI.BehaviorTree {
    public class Selector : InteriorNode {
        public override Result Run() {
            if (CurrentChild >= Children.Count) {
                CurrentChild = 0;
                return Result.FAILURE;
            }

            Result res = Children[CurrentChild].Run();
            switch (res) {
                case Result.FAILURE:
                    CurrentChild++;
                    break;
                case Result.SUCCESS:
                    CurrentChild = 0;
                    return Result.SUCCESS;
                case Result.IN_PROGRESS:
                    // Do nothing, just return InProgress
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Result.IN_PROGRESS;
        }

        public Selector(IEnumerable<BehaviorTree> children) : base(children) { }

        public override BehaviorTree Copy() {
            return new Selector(CopyChildren());
        }
    }
}
