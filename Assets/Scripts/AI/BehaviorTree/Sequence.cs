using System;
using System.Collections.Generic;


namespace CMPM.AI.BehaviorTree {
    public class Sequence : InteriorNode {
        public Sequence(IEnumerable<BehaviorTree> children) : base(children) { }

        public override Result Run() {
            if (CurrentChild >= Children.Count) {
                CurrentChild = 0;
                return Result.SUCCESS;
            }

            Result res = Children[CurrentChild].Run();
            switch (res) {
                case Result.FAILURE:
                    CurrentChild = 0;
                    return Result.FAILURE;
                case Result.SUCCESS:
                    CurrentChild++;
                    break;
                case Result.IN_PROGRESS:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Result.IN_PROGRESS;
        }

        public override BehaviorTree Copy() {
            return new Sequence(CopyChildren());
        }
    }
}